<template>
  <div>
    <el-row>
     <el-col :span="24">
        <el-steps :active="4" finish-status="success">
          <el-step title="1.选择通行证"></el-step>
          <el-step title="2.选择具体操作"></el-step>
          <el-step title="3.二次确认"></el-step>
          <el-step title="4.完成"></el-step>          
      </el-steps>       
     </el-col>
    </el-row>
    <div><span class="tag-group__title">操作：</span></div>
    <div class="card-box" >
    <el-col :span="12" >
        <el-tooltip  effect="dark" content="选择通行证，会实时刷新与该通行证绑定的车辆" placement="top">
            <el-select v-model="selectedPermitId"   @change="SelectPermitChange" filterable   placeholder="选择通行证">
          <el-option
            v-for="item in permit_info_list"
            :key="item.permit_id"
            :label="item.permit_name"
            :value="item.permit_id">
         </el-option>
        </el-select>
        </el-tooltip>
      </el-col>
      <el-col :span="12" :offset="12" >
           <el-button  @click="UnbindPermitClick">解除绑定</el-button>
          <el-button  @click="BindPermitClick">绑定通行证</el-button>          
          <el-button  @click="EditPermitClick">编辑通行证</el-button>               
          <el-button  @click="DeletePermitClick" type="danger">删除通行证</el-button>   
          <el-button  @click="AddNewPermitClick" type="success">新增通行证</el-button>   
         </el-col>
    </div>
    <el-dialog title="新增通行证"
    :visible.sync="addNewPermitVisible"
    width="30%">
    <div>
      <span>新通行证名称</span>
      <el-input placeholder="通行证名称" v-model="newPermitName"></el-input></div>
       <span slot="footer" class="dialog-footer">
         <el-button @click="AddNewPermitCancel" type="danger">取消</el-button>
        <el-button @click="AddNewPermitConfirm" type="success">确认</el-button>
       </span>
      </el-dialog>
    <el-col :span="24">
      <div>
      <div><span class="tag-group__title">已选择车辆</span></div>      
      <el-col :span="12" >
        <el-tag
          :key="truckTag.truck_id"
          v-for="truckTag in selectedTrucks"
          closable
          @close="handleClose(truckTag)">
           {{truckTag.truck_num}}
        </el-tag>
      </el-col>
    </div>
    </el-col>
  <div>
    <span class="tag-group__title">已绑定该通行证</span>
  </div>
  <div class="card-box">
    <el-card  class="active-card"
    v-for="item in PermitBindTruckLst"
    :key="item.truck_id"    
    ><div >
      <el-button class="card-button" v-text="item.truck_num" @click="bindedTruckClick(item.truck_id,item.truck_num)"></el-button>      
      </div>    
    </el-card>
  </div>
<div class="tag-group">
  <div><span class="tag-group__title">未绑定该通行证</span></div>
  <div class="card-box">
    <el-card  class="stop-card"
    v-for="item in PermitUnbindTruckLst"
    :key="item.truck_id"  
    ><div >
     <el-button class="card-button" v-text="item.truck_num" @click="unbindTruckClick(item.truck_id,item.truck_num)"></el-button>
      </div>    
    </el-card>
  </div>
</div>
  </div>  
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
   export default {
     props:{
       'activeTruckInfo':Array,
     'stopTruckInfo':Array,
     'permit_info_list':Array,
     'truck_info_list':Array,
     },
    data() {
      return {
        items: [],
        operateItem:"",
        selectedPermitId:"",
        selectedTruckLst:[],
        selectedTruckIdLst:[],
        PermitBindTruckLst:[],
        PermitUnbindTruckLst:[],
        selectedTrucks:[],
        addNewPermitVisible:false,
        newPermitName:"",

      }
    },
    created(){
      if(this.permit_info_list!=null){
          if(this.truck_info_list!=null){
            this.truck_info_list.forEach((element)=>{
               
            });
          }
      }
    },
    methods:{
      //关闭 已选中的标签
      handleClose(truckTag){
         this.selectedTrucks.splice(this.selectedTrucks.indexOf(truckTag),1);
         this.selectedTruckIdLst.splice(this.selectedTruckIdLst.indexOf(truckTag.truck_id),1);
      },
      //切换操作项
      SelectPermitChange(){
         if(this.selectedPermitId!=0){
           this.selectedTrucks.length=0;
           this.selectedTruckIdLst.length=0;
           this.PermitBindTruckLst=[];
           this.PermitUnbindTruckLst=[];
            axios
          .get(Common.GetApiUrl('api/permit/query_bind?permit_id=')+this.selectedPermitId)
          .then((response)=>{
            if(response.data.code!=1000){
              this.$message.error(response.data.message);
              return;
            }
            console.log("truck/set_active"+response.data.data);
            
            response.data.data.forEach(element=>{
              
               this.PermitBindTruckLst.push({
                 'truck_num':element["truck_num"],
                 'truck_id':element["truck_id"],
               });                 
            });
            this.PermitBindTruckLst.sort(function(a,b){return a-b});
            console.log("index="+this.PermitBindTruckLst.indexOf(1))
            
            this.truck_info_list.forEach(element=>{
              var tmpTruckId=parseInt(element.truck_id);
              if(this.PermitBindTruckLst.findIndex(function(item){ return item.truck_id==element.truck_id})==-1){
                this.PermitUnbindTruckLst.push({
                  'truck_num':element["truck_num"],
                 'truck_id':element["truck_id"],
                });
              }
            });
            this.PermitUnbindTruckLst.sort(function(a,b){return a-b});
      })
         }
      },
      //点击已绑定通行证
      bindedTruckClick(truck_id,truck_num){
          console.log(truck_id);
        if(this.selectedTrucks.findIndex((value)=>value.truck_id==truck_id)>=0){          
           this.$message({ message:"请勿重复选择",type:'warning' })
           return;
        }
        this.selectedTrucks.push({"truck_id":truck_id,"truck_num":truck_num});
        this.selectedTruckIdLst.push(truck_id);
      },
      //点击未绑定通行证
      unbindTruckClick(truck_id,truck_num){
        console.log(truck_id);
        if(this.selectedTrucks.findIndex((value)=>value.truck_id==truck_id)>=0){          
           this.$message({ message:"请勿重复选择",type:'warning' })
           return;
        }
         this.selectedTrucks.push({"truck_id":truck_id,"truck_num":truck_num});
         this.selectedTruckIdLst.push(truck_id);
      },
      //点击 停运中的车辆
      stopTruckClick(){

      },
      //解除绑定
      UnbindPermitClick(){
        console.log(this.selectedTrucks)
        if(!this.CheckSelectedTrucks(0)) return;
        //获取选中的车辆
      axios
      .get(Common.GetApiUrl('api/permit/unbind_permit?permit_id=')+this.selectedPermitId+"&truckLst="+this.selectedTruckIdLst.toString())
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log(Common.GetApiUrl('api/permit/unbind_permit?permit_id=')+this.selectedPermitId+"&truckLst="+this.selectedTruckIdLst);
        this.$router.go(0);  
      })
      },
      //新增绑定通行证
      BindPermitClick(stopType){        
        if(!this.CheckSelectedTrucks(0)) return;
        
        axios
      .get(Common.GetApiUrl('api/permit/bind_permit?permit_id=')+this.selectedPermitId+"&truckLst="+this.selectedTruckIdLst.toString())
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log(Common.GetApiUrl('api/permit/bind_permit?permit_id=')+this.selectedPermitId+"&truckLst="+this.selectedTruckIdLst);
        this.$router.go(0);  
      })
      },
      //编辑通行证
      EditPermitClick(){
        this.$message.error("功能未开放，敬请期待"); 
          return;
         if(!this.CheckSelectedTrucks(1)) return;

      },
      //新增通行证
      AddNewPermitClick(){
        this.addNewPermitVisible=true;
        
      },
      AddNewPermitConfirm(){
          if(this.newPermitName==""){
            this.$message.error("通行证名称不能为空！"); 
          return;
        }
        axios
        .get(Common.GetApiUrl('api/permit/add?newPermit=')+this.newPermitName)
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          console.log("api/permit/add?newPermit "+response.data.data);
          this.$router.go(0);  
        })
      },
      AddNewPermitCancel(){
        this.addNewPermitVisible=false;
      },
      //删除通行证
      DeletePermitClick(){
       
        
         axios
      .get(Common.GetApiUrl('api/permit/delete?permit_id=')+this.selectedPermitId)
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("api/permit/delete"+response.data.data);
        this.$router.go(0);  
      })
      },
      //判断选中车辆的数目是否符合需求  needNum=需求的数量限制，0=不限制，1=只允许1个
      CheckSelectedTrucks(needNum){
          if(needNum==0&&this.selectedTrucks.length==0){
            this.$message({ message:"当前未选择任何车辆",type:'warning' })
            return false;
          }
          if(needNum==1){
            if(this.selectedTrucks.length==0){
              this.$message({ message:"当前未选择任何车辆",type:'warning' })
              return false;
            }else if(this.selectedTrucks.length>1){
              this.$message({ message:"只能选中一个车辆并进行操作",type:'warning' })
              return false;
            }
          }
          return true;
      }
    }
  }

</script>
<style scoped>
 .el-tag{
   width: 60px;
   height: 30px;
   margin-right: 20px;
   margin-top:10px;
 }
 .card-box{
   display:flex;
   flex-direction:row;
   flex-wrap:wrap;
   justify-content:flex-start
 }
 .el-card{
   width:80px;
   height: 40px;
   margin-right: 20px;
   margin-top:10px;
   font-size: 120%;
   
   text-align: center;
   display:flex;
   justify-content:center;
   align-items:center;
 }
 .active-card{
   background-color: lightgreen;
 }
 .stop-card{
   background-color: salmon;
 }
 .el-card .el-button{
   background-color: transparent;
   border: none;
   font-size: 120%;
   text-align: center;
 }
 .el-input{
   width:350px;
 }
</style>