import * as monaco from 'monaco-editor'

const findNodeCompletionItem = {
    label: 'findNode',
    kind: monaco.languages.CompletionItemKind.Function,
    insertText:"findNode",
    documentation: {
        value: '查找指定的节点对象',
        // 添加参数说明
        details: [
            {
                label: 'parentNode',
                documentation: '从哪个节点开始查找，顶级节点可以用 this.$data 表示',
            },
            {
                label: 'nodeType',
                documentation: '查找的节点类型',
            },
            {
                label: 'name',
                documentation: '节点的名称, null表示任何名称',
            },
        ],
    },
};

const suggestionArr = [findNodeCompletionItem];

monaco.languages.registerCompletionItemProvider('html', {
    provideCompletionItems: (model: any, position: any) => {
        return { suggestionArr };//直接提供，不用判断


        const line = position.lineNumber // 当前行数
        const column = position.column // 当前列数
        const content = model.getLineContent(line) // 当前行全部内容
        const preContent = content.substr(0, column - 1);//光标往前的内容

        const match = preContent.match(/\w+$/);
        var word = match ? match[0] : '';

        //console.log(word, preContent);
        var suggestions = <any[]>[];
        suggestionArr.forEach(m => {
            if (m.label.startsWith(word)) {
                suggestions.push(m);
            }
        });


        return { suggestions }
    },
    //triggerCharacters: ['['] // 触发语法推荐的提示符，设置为[， 也可以不设置，这样用户只能通过输入 xx 的方式来激活。
});

monaco.languages.registerSignatureHelpProvider("html", {
    signatureHelpTriggerCharacters: ["(", ","],
    provideSignatureHelp: (model:any, position:any, token:any) => {

      return {
        dispose: () => {},
        value: {
          activeParameter: 0,
          activeSignature: 0,
          signatures: [
            {
                label: 'findNode(parentNode , nodeType:number , name:string?)',
                documentation: `findNode(parentNode , nodeType:number , name:string?) 查找指定的节点对象
                parentNode: 从哪个节点开始查找，顶级节点可以用 this.$data 表示
                nodeType: 查找的节点类型
                name: 节点的名称, null表示任何名称
                `,
                parameters: [
                    {
                        label: 'parentNode',
                        documentation: '从哪个节点开始查找，顶级节点可以用 this.$data 表示',
                    },
                    {
                        label: 'nodeType',
                        documentation: '查找的节点类型',
                    },
                    {
                        label: 'name',
                        documentation: '节点的名称, null表示任何名称',
                    },
                ],
              }
          ],
        },
      };
    },
  });
