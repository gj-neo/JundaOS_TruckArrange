<template>
  <el-table
    :data="tableData"
    stripe
    style="width: 100%">
    <el-table-column
      prop="date"
      label="序号"
      width="50">
    </el-table-column>
    <el-table-column
      prop="name"
      label="工地名称"
      width="180">
    </el-table-column>
    <el-table-column
      prop="address"
      label="总排班次数">
    </el-table-column>
     <el-table-column
      prop="address"
      label="总计派出车次">
    </el-table-column>
     <el-table-column
      prop="address"
      label="自动排班次数">
    </el-table-column>
     <el-table-column
      prop="address"
      label="派出累计车次">
    </el-table-column>
     <el-table-column
      prop="address"
      label="手动排班次数">
    </el-table-column>
     <el-table-column
      prop="address"
      label="派出累计车次">
    </el-table-column>
  </el-table>
</template>
<script>
  export default {
    data() {
      return {
        tableData: [ {
          orderId: '2',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1517 弄'
        }, {
          orderId: '3',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1519 弄'
        }, {
          orderId: '4',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1516 弄'
        }]
      }
    }
  }
</script>
<style scoped>
 
</style>