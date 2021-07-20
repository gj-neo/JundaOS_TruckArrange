<template>
<div>
  <!--标题栏-->
    <el-row>
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">统计-任务</el-divider>
        </div>
      </el-col>
      <el-col :span="16">
         <el-select v-model="value" filterable placeholder="请选择工地">
          <el-option
            v-for="item in from_site_options"
            :key="item.site_id"
            :label="item.value"
            :value="item.id">
          </el-option>
        </el-select>
        <el-button icon="el-icon-search" circle></el-button>        
      </el-col>
      <el-col :span="4">
        <el-button>新增工地</el-button>
      </el-col>
    </el-row>
    <el-row>
      <el-col :span="24">
        <statistic_task_table></statistic_task_table>
      </el-col>
    </el-row>
    <!--结果栏-->
    <el-row>
      <el-col :span="24">
      </el-col>
    </el-row>
</div>
</template>
<script>
import statistic_task_table from '../components/statistic_task_table'
export default {
    name: 'teset',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
      }
    },
    components:{      
      'statistic_task_table': statistic_task_table,
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