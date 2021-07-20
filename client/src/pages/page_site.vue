<template>
<div>
    <el-row>
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">工地管理</el-divider>
        </div>
      </el-col>      
    </el-row>
    <el-row>
      <!--提交表单-->
     <el-dialog :title="submit_form_title"  :visible.sync="submit_form_visible">
      <el-form :model="form">
        <el-form-item label="工地名称" :label-width="formLabelWidth">
          <el-input v-model="submit_site_name" autocomplete="off" ></el-input>
        </el-form-item>        
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="submit_form_visible = false">取 消</el-button>
        <el-button type="primary" @click="Submit_confirm_click">确 定</el-button>
      </div>
    </el-dialog>
      <el-col :span="6" :offset="18" >          
          <el-button  @click="AddNewFromSite" type="success">新增工地</el-button>      
          <el-button  @click="AddNewToSite" type="success">新增消纳场</el-button>   
         </el-col>
    </el-row>
    <el-row>
      <el-col :span="11">
        <el-table
    :data="from_site_table_data"
    stripe
    style="width: 100%">
    <el-table-column
     type="index"
      :index="site_table_index_method"
      width="40">
    </el-table-column>
    <el-table-column
      prop="site_name"
      label="工地名称"
      width="320">
    </el-table-column>
     <el-table-column
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="primary"
          @click="handleModified_fromSite(scope.$index, scope.row)">修改</el-button>
           <el-button
          size="mini"
          type="danger"
          @click="handleDelete_fromSite(scope.$index, scope.row)">删除</el-button>
          
      </template>
    </el-table-column>
  </el-table>
      </el-col>
      <el-col :span="11">
        <el-table
    :data="to_site_table_data"    
    stripe
    style="width: 100%">
    <el-table-column
     type="index"
      :index="site_table_index_method"
      width="40">
    </el-table-column>
    <el-table-column
      prop="site_name"
      label="消纳场名称"
      width="320">
    </el-table-column>
     <el-table-column
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="primary"
          @click="handleModified_toSite(scope.$index, scope.row)">修改</el-button>
           <el-button
          size="mini"
          type="danger"
          @click="handleDelete_toSite(scope.$index, scope.row)">删除</el-button>
          
      </template>
    </el-table-column>
  </el-table>
      </el-col>
    </el-row>
</div>
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
import site_submit_dialog from '../components/dialogs/site_submit_dialog'

export default {
    name: 'site',
    data(){
      return {        
        submit_form_visible:false,
        from_site_table_data:[],
        to_site_table_data:[],
        display_type:Number,
        submit_form_title:'',
        form: {
          name: '',
          region: '',
          date1: '',
          date2: '',
          delivery: false,
          type: [],
          resource: '',
          desc: ''
        },
        formLabelWidth: '120px',
        submit_site_name:'',
        submit_site_id:'',
        submit_form_operate_type:'',
      }
    },
    components:{
      
    },
    created(){
      //获取工地列表
      axios
      .get(Common.GetApiUrl('api/site/query')+Common.Create_CommonParam_HttpGet("all",null))
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log(response.data.data);
        this.from_site_table_data=[];
        response.data.data.forEach(element => {
          if(element.site_type==1){
            this.from_site_table_data.push({
              'site_id':element.site_id,
             'site_name':element.site_name,
             'site_type':element.site_type,
           });
          }
          else{
            this.to_site_table_data.push({
               'site_id':element.site_id,
             'site_name':element.site_name,
             'site_type':element.site_type,
           });
          }
           
        });    
      })
    },
    methods:{
      //确认删除
      open(index,row) {
        
      },
      //修改工地
      handleModified_fromSite(index,row){
        console.log(this.from_site_table_data[index].site_id)
        console.log(this.from_site_table_data[index].site_name)
        this.submit_form_title="修改工地";
        this.submit_form_visible=true;
        this.submit_form_operate_type="modified_from_site"
        this.submit_site_name=this.from_site_table_data[index].site_name;
        this.submit_site_id=this.from_site_table_data[index].site_id;
      },
      //修改消纳场
      handleModified_toSite(index,row){
        this.submit_form_title="修改消纳场";
        this.submit_form_visible=true;
        this.submit_form_operate_type="modified_to_site";
        this.submit_site_name=this.to_site_table_data[index].site_name;
        this.submit_site_id=this.to_site_table_data[index].site_id;
      },
      //删除工地
      handleDelete_fromSite(index,row){
          var tipStr='此操作将永久删除关于:'+this.from_site_table_data[index].site_name+'的所有数据，请确认是否继续操作？'
          this.$confirm(tipStr, '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            //发送get请求
              axios
              .get(Common.GetApiUrl('api/site/delete')+"?site_id="+this.from_site_table_data[index].site_id)
              .then((response)=>{
                if(response.data.code!=1000){
                  //console.log(response)
                  this.$message.error(response.data.message);
                  return;
                }         
                this.$message.success('删除成功')
                this.$router.go(0);  
              })            
          }).catch(() => {
            this.$message({
              type: 'info',
              message: '已取消删除'
            });          
          });
      },
      //删除消纳场
      handleDelete_toSite(index,row){
          var tipStr='此操作将永久删除关于:'+this.to_site_table_data[index].site_name+'的所有数据，请确认是否继续操作？'
          this.$confirm(tipStr, '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            //发送get请求
              axios
              .get(Common.GetApiUrl('api/site/delete')+"?site_id="+this.to_site_table_data[index].site_id)
              .then((response)=>{
                if(response.data.code!=1000){
                  //console.log(response)
                  this.$message.error(response.data.message);
                  return;
                }         
                this.$message.success('删除成功')
                this.$router.go(0);  
              })            
          }).catch(() => {
            this.$message({
              type: 'info',
              message: '已取消删除'
            });          
          });
      },
      //新增工地
      AddNewFromSite(){
        this.submit_form_title="新增工地";
        this.submit_form_visible=true;
        this.submit_form_operate_type="add_new_site";
        this.submit_site_name='';
        this.submit_site_id=0;
      },
      AddNewToSite(){
          this.submit_form_title="新增消纳场";
        this.submit_form_visible=true;
        this.submit_form_operate_type="add_to_site";
        this.submit_site_name='';
        this.submit_site_id=0;
      },
      //弹出框-确认提交  
      Submit_confirm_click(){        
        var strApiUrl='';
        console.log(this.submit_form_operate_type)
        if(this.submit_form_operate_type=="add_new_site"){
            if(this.submit_site_name==''){
              this.$message.error("请输入要增加的工地名称"); return;
            }
            strApiUrl=Common.GetApiUrl('api/site/add')+"?site_name="+this.submit_site_name+"&site_type=1";
        }
        else if(this.submit_form_operate_type=="add_to_site"){
            if(this.submit_site_name==''){
              this.$message.error("请输入要增加的消纳场名称"); return;
            }
           strApiUrl=Common.GetApiUrl('api/site/add')+"?site_name="+this.submit_site_name+"&site_type=2";
        }
        else if(this.submit_form_operate_type=="modified_from_site"){
            if(this.submit_site_name==''){
              this.$message.error("工地名称不能为空"); return;
            }           
           strApiUrl=Common.GetApiUrl('api/site/update_site_name?site_id=')+this.submit_site_id+"&site_name="+this.submit_site_name;

        }
        else if(this.submit_form_operate_type=="modified_to_site"){
             if(this.submit_site_name==''){
              this.$message.error("消纳场名称不能为空"); return;
            }
            strApiUrl=Common.GetApiUrl('api/site/update_site_name?site_id=')+this.submit_site_id+"&site_name="+this.submit_site_name;
        }
        console.log(strApiUrl);
        //发送get请求
        axios
        .get(strApiUrl)
        .then((response)=>{
          if(response.data.code!=1000){
            //console.log(response)
            this.$message.error(response.data.message);
            return;
          }         
          this.$message.success('操作成功')
          this.$router.go(0);  
        })
        this.submit_form_visible=false;
      },
      DisplayFromSiteClick(){
          this.from_site_table_data.filter(function(x){
             return x.site_type=="工地"
          });
      },
      DisplayToSiteClick(){
          this.to_site_table_data.filter(function(x){
             return x.site_type=="消纳场"
          });
      },
       site_table_index_method(index){
        return index+1;
      }
}
}
</script>
<style >
.el-row {
    margin-bottom: 20px;
    text-align: left;
  }
  .el-col{
    margin-right: 20px;
  }
</style>