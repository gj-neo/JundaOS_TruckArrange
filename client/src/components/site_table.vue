<template>
  <el-table
    :data="site_table_data"
    stripe
    style="width: 100%">
    <el-table-column
     type="index"
      :index="schedule_task_table_index_method"
      width="50">
    </el-table-column>
    <el-table-column
      prop="site_name"
      label="工地名称"
      width="180">
    </el-table-column>
     <el-table-column
      prop="site_type"
      label="工地类型">
    </el-table-column>    
     <el-table-column
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="primary"
          @click="handleModified(scope.$index, scope.row)">修改</el-button>
           <el-button
          size="mini"
          type="danger"
          @click="handleDelete(scope.$index, scope.row)">删除</el-button>
           <el-button
          size="mini"
          type="danger"          
          @click="handleAdd(scope.$index, scope.row)">新增</el-button>
      </template>
    </el-table-column>
  </el-table>
</template>
<script>
  export default {
    name:"site_table",
    props:{
      'site_table_data':Array,
    },
    methods:{
      schedule_task_table_index_method(index){
        return index+1;
      },
    },
    data() {
      return {
        tableData: [{
          date: '1',
          name: '东方高炉北高铁站',
          address: '上海市普陀区金沙江路 1518 弄'
        }, {
          date: '2',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1517 弄'
        }, {
          date: '3',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1519 弄'
        }, {
          date: '4',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1516 弄'
        }]
      }
    }
  }
</script>
<style scoped>
 
</style>