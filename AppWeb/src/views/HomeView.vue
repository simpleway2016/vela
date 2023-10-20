<script setup lang="ts">

import { ref, onMounted } from "vue"

</script>

<template>
    <div class="pageContent">
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">Data Tables</h1>
            <ol class="breadcrumb">
                <li><a href="index.html">Dashboard</a></li>
                <li><a href="#">Tables</a></li>
                <li class="active">Data Tables</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right">
                <div class="btn-group" role="group" aria-label="...">
                    <a href="index.html" class="btn btn-light">Dashboard</a>
                    <a href="#" class="btn btn-light"><i class="fa fa-refresh"></i></a>
                    <a href="#" class="btn btn-light"><i class="fa fa-search"></i></a>
                    <a href="#" class="btn btn-light" id="topstats"><i class="fa fa-line-chart"></i></a>
                </div>
            </div>
            <!-- End Page Header Right Div -->

        </div>
        <!-- End Page Header -->


        <!-- Start Row -->
        <div class="row">

            <!-- Start Panel -->
            <div class="col-md-12">
                <div class="panel panel-default">
                    <!-- <div class="panel-title">
                        DAtaTables
                    </div> -->
                    <div class="panel-body table-responsive">

                        <table id="example0" class="table display">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Position</th>
                                    <th>Office</th>
                                    <th>Age</th>
                                    <th>Start date</th>
                                    <th>Salary</th>
                                </tr>
                            </thead>

                            <tfoot>
                                <tr>
                                    <th>Name</th>
                                    <th>Position</th>
                                    <th>Office</th>
                                    <th>Age</th>
                                    <th>Start date</th>
                                    <th>Salary</th>
                                </tr>
                            </tfoot>

                            <tbody>
                                <tr>
                                    <td>Tiger Nixon</td>
                                    <td>System Architect</td>
                                    <td>Edinburgh</td>
                                    <td>61</td>
                                    <td>2011/04/25</td>
                                    <td>$320,800</td>
                                </tr>
                                <tr>
                                    <td>Ashton Cox</td>
                                    <td>Junior Technical Author</td>
                                    <td>San Francisco</td>
                                    <td>66</td>
                                    <td>2009/01/12</td>
                                    <td>$86,000</td>
                                </tr>
                                <tr>
                                    <td>Cedric Kelly</td>
                                    <td>Senior Javascript Developer</td>
                                    <td>Edinburgh</td>
                                    <td>22</td>
                                    <td>2012/03/29</td>
                                    <td>$433,060</td>
                                </tr>
                                <tr>
                                    <td>Airi Satou</td>
                                    <td>Accountant</td>
                                    <td>Tokyo</td>
                                    <td>33</td>
                                    <td>2008/11/28</td>
                                    <td>$162,700</td>
                                </tr>
                                <tr>
                                    <td>Brielle Williamson</td>
                                    <td>Integration Specialist</td>
                                    <td>New York</td>
                                    <td>61</td>
                                    <td>2012/12/02</td>
                                    <td>$372,000</td>
                                </tr>
                                <tr>
                                    <td>Herrod Chandler</td>
                                    <td>Sales Assistant</td>
                                    <td>San Francisco</td>
                                    <td>59</td>
                                    <td>2012/08/06</td>
                                    <td>$137,500</td>
                                </tr>
                                <tr>
                                    <td>Rhona Davidson</td>
                                    <td>Integration Specialist</td>
                                    <td>Tokyo</td>
                                    <td>55</td>
                                    <td>2010/10/14</td>
                                    <td>$327,900</td>
                                </tr>
                                <tr>
                                    <td>Colleen Hurst</td>
                                    <td>Javascript Developer</td>
                                    <td>San Francisco</td>
                                    <td>39</td>
                                    <td>2009/09/15</td>
                                    <td>$205,500</td>
                                </tr>
                                <tr>
                                    <td>Sonya Frost</td>
                                    <td>Software Engineer</td>
                                    <td>Edinburgh</td>
                                    <td>23</td>
                                    <td>2008/12/13</td>
                                    <td>$103,600</td>
                                </tr>
                                <tr>
                                    <td>Jena Gaines</td>
                                    <td>Office Manager</td>
                                    <td>London</td>
                                    <td>30</td>
                                    <td>2008/12/19</td>
                                    <td>$90,560</td>
                                </tr>
                                <tr>
                                    <td>Quinn Flynn</td>
                                    <td>Support Lead</td>
                                    <td>Edinburgh</td>
                                    <td>22</td>
                                    <td>2013/03/03</td>
                                    <td>$342,000</td>
                                </tr>
                                <tr>
                                    <td>Charde Marshall</td>
                                    <td>Regional Director</td>
                                    <td>San Francisco</td>
                                    <td>36</td>
                                    <td>2008/10/16</td>
                                    <td>$470,600</td>
                                </tr>
                                <tr>
                                    <td>Haley Kennedy</td>
                                    <td>Senior Marketing Designer</td>
                                    <td>London</td>
                                    <td>43</td>
                                    <td>2012/12/18</td>
                                    <td>$313,500</td>
                                </tr>
                                <tr>
                                    <td>Tatyana Fitzpatrick</td>
                                    <td>Regional Director</td>
                                    <td>London</td>
                                    <td>19</td>
                                    <td>2010/03/17</td>
                                    <td>$385,750</td>
                                </tr>
                                <tr>
                                    <td>Michael Silva</td>
                                    <td>Marketing Designer</td>
                                    <td>London</td>
                                    <td>66</td>
                                    <td>2012/11/27</td>
                                    <td>$198,500</td>
                                </tr>
                                <tr>
                                    <td>Paul Byrd</td>
                                    <td>Chief Financial Officer (CFO)</td>
                                    <td>New York</td>
                                    <td>64</td>
                                    <td>2010/06/09</td>
                                    <td>$725,000</td>
                                </tr>
                                <tr>
                                    <td>Gloria Little</td>
                                    <td>Systems Administrator</td>
                                    <td>New York</td>
                                    <td>59</td>
                                    <td>2009/04/10</td>
                                    <td>$237,500</td>
                                </tr>
                                <tr>
                                    <td>Bradley Greer</td>
                                    <td>Software Engineer</td>
                                    <td>London</td>
                                    <td>41</td>
                                    <td>2012/10/13</td>
                                    <td>$132,000</td>
                                </tr>
                                <tr>
                                    <td>Dai Rios</td>
                                    <td>Personnel Lead</td>
                                    <td>Edinburgh</td>
                                    <td>35</td>
                                    <td>2012/09/26</td>
                                    <td>$217,500</td>
                                </tr>
                                <tr>
                                    <td>Jenette Caldwell</td>
                                    <td>Development Lead</td>
                                    <td>New York</td>
                                    <td>30</td>
                                    <td>2011/09/03</td>
                                    <td>$345,000</td>
                                </tr>
                                <tr>
                                    <td>Yuri Berry</td>
                                    <td>Chief Marketing Officer (CMO)</td>
                                    <td>New York</td>
                                    <td>40</td>
                                    <td>2009/06/25</td>
                                    <td>$675,000</td>
                                </tr>
                                <tr>
                                    <td>Caesar Vance</td>
                                    <td>Pre-Sales Support</td>
                                    <td>New York</td>
                                    <td>21</td>
                                    <td>2011/12/12</td>
                                    <td>$106,450</td>
                                </tr>
                                <tr>
                                    <td>Doris Wilder</td>
                                    <td>Sales Assistant</td>
                                    <td>Sidney</td>
                                    <td>23</td>
                                    <td>2010/09/20</td>
                                    <td>$85,600</td>
                                </tr>
                                <tr>
                                    <td>Angelica Ramos</td>
                                    <td>Chief Executive Officer (CEO)</td>
                                    <td>London</td>
                                    <td>47</td>
                                    <td>2009/10/09</td>
                                    <td>$1,200,000</td>
                                </tr>
                                <tr>
                                    <td>Gavin Joyce</td>
                                    <td>Developer</td>
                                    <td>Edinburgh</td>
                                    <td>42</td>
                                    <td>2010/12/22</td>
                                    <td>$92,575</td>
                                </tr>
                                <tr>
                                    <td>Jennifer Chang</td>
                                    <td>Regional Director</td>
                                    <td>Singapore</td>
                                    <td>28</td>
                                    <td>2010/11/14</td>
                                    <td>$357,650</td>
                                </tr>
                                <tr>
                                    <td>Brenden Wagner</td>
                                    <td>Software Engineer</td>
                                    <td>San Francisco</td>
                                    <td>28</td>
                                    <td>2011/06/07</td>
                                    <td>$206,850</td>
                                </tr>
                                <tr>
                                    <td>Fiona Green</td>
                                    <td>Chief Operating Officer (COO)</td>
                                    <td>San Francisco</td>
                                    <td>48</td>
                                    <td>2010/03/11</td>
                                    <td>$850,000</td>
                                </tr>
                                <tr>
                                    <td>Shou Itou</td>
                                    <td>Regional Marketing</td>
                                    <td>Tokyo</td>
                                    <td>20</td>
                                    <td>2011/08/14</td>
                                    <td>$163,000</td>
                                </tr>
                                <tr>
                                    <td>Michelle House</td>
                                    <td>Integration Specialist</td>
                                    <td>Sidney</td>
                                    <td>37</td>
                                    <td>2011/06/02</td>
                                    <td>$95,400</td>
                                </tr>
                                <tr>
                                    <td>Suki Burks</td>
                                    <td>Developer</td>
                                    <td>London</td>
                                    <td>53</td>
                                    <td>2009/10/22</td>
                                    <td>$114,500</td>
                                </tr>
                            </tbody>
                        </table>


                    </div>

                </div>
            </div>
            <!-- End Panel -->
        </div>
        <!-- End Row -->

        <!-- Start 表单  -->
        <div class="row">

            <div class="col-md-12">
                <div class="panel panel-default">

                    <div class="panel-title">
                        编辑服务器信息
                        <ul class="panel-tools">
                            <li><a class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                        </ul>
                    </div>

                    <div class="panel-body">
                        <form class="form-horizontal">

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">Static Field</label>
                                <div class="col-sm-10">
                                    <p class="form-control-static">email@example.com</p>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">Normal</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="input002">
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input001" class="col-sm-2 control-label form-label">Help Text</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="input001">
                                    <span id="helpBlock" class="help-block">A block of help text that breaks onto a new line
                                        and may extend beyond one line.</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input2" class="col-sm-2 control-label form-label">Rounded</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control form-control-radius" id="input2">
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input3" class="col-sm-2 control-label form-label">Password</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" id="input3" placeholder="Password">
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input4" class="col-sm-2 control-label form-label">Disable</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="input4" placeholder="Disable Input"
                                        disabled>
                                </div>
                            </div>

                            <div class="form-group has-success">
                                <label for="input6" class="col-sm-2 control-label form-label">Input with success</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="input6">
                                </div>
                            </div>

                            <div class="form-group has-warning">
                                <label for="input7" class="col-sm-2 control-label form-label">Input with warning</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="input7">
                                </div>
                            </div>

                            <div class="form-group has-error">
                                <label for="input8" class="col-sm-2 control-label form-label">Input with warning</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="input8">
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">Readonly input</label>
                                <div class="col-sm-10">
                                    <input class="form-control" type="text" placeholder="Readonly input here…" readonly>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="exampleInputAmount" class="col-sm-2 control-label form-label">With Icons</label>
                                <div class="col-sm-10">
                                    <label class="sr-only" for="exampleInputAmount">Amount (in dollars)</label>
                                    <div class="input-group">
                                        <div class="input-group-addon"><i class="fa fa-usd"></i></div>
                                        <input type="text" class="form-control" id="exampleInputAmount"
                                            placeholder="Amount">
                                        <div class="input-group-addon">.00</div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="exampleInputAmount2" class="col-sm-2 control-label form-label">Another With
                                    Icons</label>
                                <div class="col-sm-10">
                                    <label class="sr-only" for="exampleInputAmount2">Login</label>
                                    <div class="input-group">
                                        <div class="input-group-addon"><i class="fa fa-user"></i></div>
                                        <input type="text" class="form-control" id="exampleInputAmount2"
                                            placeholder="Username">
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">Textarea</label>
                                <div class="col-sm-10">
                                    <textarea class="form-control" rows="3" id="textarea1"
                                        placeholder="Type your message..."></textarea>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">With a Line</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control form-control-line"
                                        placeholder="What is your name?">
                                </div>
                            </div>


                        </form>

                    </div>

                </div>
            </div>

        </div>
        <!-- End 表单 -->
    </div>
</template>


