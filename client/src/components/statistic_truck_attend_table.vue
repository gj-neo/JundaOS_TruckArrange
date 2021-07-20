<template>
  <div>
    <el-table
    :data="tableData"
    stripe
    style="width: 100%">
    <el-table-column
      prop="date"
      label="车辆编号"
      width="50">
    </el-table-column>
    <el-table-column
      prop="name"
      label="出勤次数"
      width="180">
    </el-table-column>
    <el-table-column
      prop="address"
      label="累计有效出勤时间（天）">
    </el-table-column>
     <el-table-column
      prop="address"
      label="出勤率">
    </el-table-column>
  </el-table>
  </div>
</template>
<script>
  export default {
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