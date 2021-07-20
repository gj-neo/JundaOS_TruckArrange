<template>
  <div>
    <el-row>
     <el-col :span="24">
        <el-steps :active="4" finish-status="success">
          <el-step title="1.选择车辆"></el-step>
          <el-step title="2.选择按钮并点击"></el-step>
          <el-step title="3.二次确认"></el-step>
          <el-step title="4.完成"></el-step>          
      </el-steps>       
     </el-col>
    </el-row>
    <div><span class="tag-group__title">操作：</span></div>
    <div class="card-box">
    <el-col :span="12" >
        <el-tag
          :key="truckTag.truck_id"
          v-for="truckTag in selectedTrucks"
          closable
          @close="handleClose(truckTag)">
           {{truckTag.truck_num}}
        </el-tag>
      </el-col>
      <el-col :span="18" :offset="6" >          
          <el-button  @click="SetTruckActiveClick" type="primary">恢复运营(需选中车辆)</el-button>
           <el-button  @click="SetAllStopTruckActiveClick" type="warning">恢复运营(所有停运车辆)</el-button>
          <el-button  @click="SetTruckStopClick(1)" type="info">停运</el-button>
          <el-button  @click="SetTruckStopClick(2)" type="info">请假</el-button>
          <el-button  @click="OpenTruckEdit">编辑信息</el-button>   
          <el-button  @click="AddNewTruck" type="primary">新增车辆</el-button>      
          <el-button  @click="DeleteTruck" type="danger">删除车辆</el-button>   
         </el-col>
    </div>
    <el-dialog :title="operationName" :visible.sync="dialogVisble" center width="400px">
        <div><span class="tag-group__title">请确认操作！</span></div>
        <span slot="footer" class="dialog-footer">
          <el-button @click="Btn_DialogCancel_Clicked">取消</el-button>
          <el-button type="primary" @click="Btn_DialogOK_Clicked">确定</el-button>
        
        </span>
    </el-dialog>
    <!-- <el-form :model="AddNewTruckForm" :rules="rules">
      <el-form-item label="车辆编号" prop="name" :v-if="addNewTruckDialogVisible">
        <el-input v-model="AddNewTruckRule"></el-input>
      </el-form-item>
      <el-form-item>
        <el-input v-model=
      </el-form-item>
    </el-form> -->
    <el-dialog  :visible.sync="addNewTruckDialogVisible" center width="400px">
        <div>
          <span>车辆编号</span>
          <el-input placeholder="请输入车辆编号" v-model="newTruckId"></el-input>
        </div>  
        <div>
          <span>司机姓名</span>
           <el-input placeholder="请输入司机姓名" v-model="newTruckDriverName"></el-input>
        </div>
        <div>
          <span>车牌号</span>
           <el-input placeholder="请输入车牌号" v-model="newTruckLicense"></el-input>
        </div>
        <div>
          <span class="tag-group__title" >选择加入时间</span><el-date-picker
          v-model="newTruckJoinTime"
          type="datetime"
          value-format="yyyy-MM-dd"
          placeholder="选择加入时间">
          </el-date-picker>
        </div>

        <span slot="footer" class="dialog-footer">
          <el-button @click="Btn_AddNewTruck_Cancel">取消</el-button>
          <el-button type="primary" @click="Btn_AddNewTruck_Confirm">确定新增</el-button>        
        </span>
    </el-dialog>
  <div><span class="tag-group__title">运营中(共{{alivedTruckCount}}辆)：</span></div>
  <div class="card-box">
    <el-card  class="active-card"
    v-for="item in activeTruckInfo"
    :key="item.truck_id"    
    @click="activeTruckClick"
    ><div >
      <el-button class="card-button" v-text="item.truck_num" @click="activeTruckClick(item.truck_id,item.truck_num)"></el-button>      
      </div>    
    </el-card>
  </div>
<div class="tag-group">
  <div><span class="tag-group__title">停运(共{{stopedTruckCount}}辆)：</span></div>
  <div class="card-box">
    <el-card  class="stop-card"
    v-for="item in stopTruckInfo"
    :key="item.truck_id"    
    @click="stopTruckClick"
    ><div >
      <el-button class="card-button" v-text="item.truck_num" @click="activeTruckClick(item.truck_id,item.truck_num)"></el-button>      

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
        'stopedTruckCount':Number,
        'alivedTruckCount':Number,
     },
    data() {
      return {
        activeTruckLst:this.activeTruckInfo,
        items: [],
        operateItem:"",
        selectedTrucks:[],
        selectedTruckIdLst:[],
        dialogVisble:false,
        addNewTruckDialogVisible:false,
        newTruckJoinTime:"",
        operationName:"",
        dialogSelectedTime:"",
        newTruckId:"",
        newTruckDriverName:"",
        newTruckLicense:"",
      }
    },
    methods:{
      //确认新增车辆
      Btn_AddNewTruck_Confirm(){
          if(this.newTruckId==""){
            this.$message({ message:"车辆编号不能为空！",type:'warning' });return;
          }
          if(this.newTruckLicense==""){
            this.$message({ message:"车牌号不能为空！",type:'warning' });return;
          }
          if(this.newTruckJoinTime==""){
            this.$message({ message:"加入时间不能为空！",type:'warning' });return;
          }
          let data={
            "truck_id":this.newTruckId,
            "truck_license":this.newTruckLicense,
            "truck_driver_name":this.newTruckDriverName,
            "truck_status":1,
            "join_time":this.newTruckJoinTime,
          }
          axios.post(Common.GetApiUrl('api/truck/add'),data)
         .then(res=>{
           if(res.data.code!=1000){
             loadingInstance.close();
            this.$message({ message:res.data.message,type:'warning' })
            return;
           }
           else{
             this.$message("新增成功");
             this.addNewTruckDialogVisible=false;
             return;
           }
           console.log("Btn_AddNewTruck_Confirm",res.data);
         })
      },
      //取消新增车辆
      Btn_AddNewTruck_Cancel(){
        this.addNewTruckDialogVisible=false;
      },
      //关闭 已选中的标签
      handleClose(truckTag){
         this.selectedTrucks.splice(this.selectedTrucks.indexOf(truckTag),1);
      },
      //切换操作项
      operateChange(){

      },
      //点击 运营中的车辆
      activeTruckClick(truck_id,truck_num){
        console.log(truck_id);
        
        ;
        
        if(this.selectedTrucks.findIndex((value)=>value.truck_id==truck_id)>=0){          
           this.$message({ message:"请勿重复选择",type:'warning' })
           return;
        }
        this.selectedTrucks.push(
          {
            "truck_id":truck_id,"truck_num":truck_num
          });
          this.selectedTruckIdLst.push(truck_id);
          console.log("activeTruckClick"+this.selectedTruckIdLst)
      },
      //点击 停运中的车辆
      stopTruckClick(){

      },
      //恢复运营
      SetTruckActiveClick(){
        this.operationName="恢复运营选中车辆";
        this.dialogVisble=true;
        
        if(!this.CheckSelectedTrucks(0)) return;
      },
      //恢复全部停运车辆
      SetAllStopTruckActiveClick(){
        if(this.stopTruckInfo.length==0){
           this.$message.error("没有停运车辆！");
           return;
        }
        this.operationName="恢复运营全部停运车辆";
        this.dialogVisble=true;
      },
      //停运  1=停运，2=请假
      SetTruckStopClick(stopType){
        
        if(!this.CheckSelectedTrucks(0)) return;
        let url;       
        // this.selectedTrucks.forEach(function(item){
        //    this.selectedTruckIdLst.push(item.truck_id);
        //   });
          console.log("this.selectedTruckIdLst"+this.selectedTruckIdLst)
        if(stopType==1){

          this.operationName="停运";
          this.dialogVisble=true;
         
         
          url=Common.GetApiUrl('api/truck/set_stop?truck_list=')+this.selectedTruckIdLst+"&stop_time="+this.dialogSelectedTime.replace("T"," ");
        }
        else {
          this.operationName="请假";
          this.dialogVisble=true;
          url=Common.GetApiUrl('api/truck/set_leave?truck_list=')+ this.selectedTruckIdLst+"&stop_time="+this.dialogSelectedTime.replace("T"," ");
        }
      //   axios
      // .get(url)
      // .then((response)=>{
      //   if(response.data.code!=1000){
      //     this.$message.error(response.data.message);
      //     return;
      //   }
      //   console.log("truck/set_active/Stop"+response.data.data);
      //   this.$router.go(0);  
      // })
      },
      //弹出框 确认
      Btn_DialogOK_Clicked()
      { 
          let url;
          if(this.operationName=="停运")
          {
            url=Common.GetApiUrl('api/truck/set_stop?truck_list=')+ this.selectedTruckIdLst;
             axios
            .get(url)
            .then((response)=>{
              if(response.data.code!=1000){
                this.$message.error(response.data.message);
                return;
              }
              console.log("truck/set_active/Stop"+response.data.data);
              this.$router.go(0);  
            })    
          }
          else if(this.operationName=="请假")
          {
             url=Common.GetApiUrl('api/truck/set_leave?truck_list=')+ this.selectedTruckIdLst;
             axios
            .get(url)
            .then((response)=>{
              if(response.data.code!=1000){
                this.$message.error(response.data.message);
                return;
              }
              console.log("truck/set_active/Stop"+response.data.data);
              this.$router.go(0);  
            })
          }
          else if(this.operationName=="恢复运营选中车辆")
          {
            
            // if(this.errorTruckLst.length>0){
            //   this.$message.warning("部分车辆("+this.errorTruckLst.Join('、')+")已经在运营中，无法再此恢复运营。");
            //   return;
            // }       
            //执行http请求
            axios
            .get(Common.GetApiUrl('api/truck/set_active?truck_list=')+ this.selectedTruckIdLst)
            .then((response)=>{
              if(response.data.code!=1000){
                this.$message.error(response.data.message);
                return;
              }
              console.log("truck/set_active"+response.data.data);
              this.$message({ message:"操作成功",type:'success' })
              this.$router.go(0);  
            })
          }
          else if(this.operationName=="恢复运营全部停运车辆")
          {
            let tmpTruckLst=[]
           this.stopTruckInfo.forEach(item=>{ tmpTruckLst.push(item.truck_id)});
            //执行http请求
            axios
            .get(Common.GetApiUrl('api/truck/set_active?truck_list=')+tmpTruckLst)
            .then((response)=>{
              if(response.data.code!=1000){
                this.$message.error(response.data.message);
                return;
              }
              console.log("truck/set_active"+response.data.data);
              this.$message({ message:"操作成功",type:'success' })
              this.$router.go(0);  
            })
          }
      },
      //弹出框，取消
      Btn_DialogCancel_Clicked()
      {
           this.dialogVisble=false;
      },
      //编辑车辆信息
      OpenTruckEdit(){
        this.$message.error("功能未开放，敬请期待"); 
          return;
         if(!this.CheckSelectedTrucks(1)) return;

      },
      //新增车辆
      AddNewTruck(){
        this.addNewTruckDialogVisible=true;
      },
      //删除车辆
      DeleteTruck(){    
        if(!this.CheckSelectedTrucks(1)) return;    
        this.$confirm('此操作将删除车辆以及该车辆的历史排班数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
          center: true
        }).then(() => {
          axios
      .get(Common.GetApiUrl('api/truck/delete?truck_id=')+this.selectedTrucks[0].truck_id)
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("truck/delete"+response.data.data);
        this.$message({
            type: 'success',
            message: '删除成功!'
          });
        this.$router.go(0);  
      })
          
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消删除'
          });
        });
         
         
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
   width:250px;
 }
</style>