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
      label="通行证名称"
      width="180">
    </el-table-column>
    <el-table-column
      prop="address"
      label="有效期限-开始">
    </el-table-column>
     <el-table-column
      prop="address"
      label="有效期限-结束">
    </el-table-column>
     <el-table-column
      prop="address"
      label="有效时刻">
    </el-table-column>
     <el-table-column
      prop="address"
      label="派车结果">
    </el-table-column>
     <el-table-column
      prop="address"
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="primary"
          @click="handleDelete(scope.$index, scope.row)">修改</el-button>
           <el-button
          size="mini"
          type="danger"
          @click="handleDelete(scope.$index, scope.row)">删除</el-button>
      </template>
    </el-table-column>
  </el-table>
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