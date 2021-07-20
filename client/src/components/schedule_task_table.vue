<template>
  <el-table
    :data="tableData"
    stripe
    style="width: 100%">
    <el-table-column
      prop="orderId"
      label="序号"
      width="50">
    </el-table-column>
    <el-table-column
      prop="from_site_name"
      label="工地"
      width="180">
    </el-table-column>
    <el-table-column
      prop="to_site_name"
      label="消纳场">
    </el-table-column>
     <el-table-column
      prop="need_permit_name"
      label="所需通行证">
    </el-table-column>
     <el-table-column
      prop="need_truck_count"
      label="派车数量">
    </el-table-column>
     <el-table-column
      prop="calc_truck_result"
      label="派车结果">
    </el-table-column>
     <el-table-column
      prop="operate"
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="danger"
          @click="handleDelete(scope.$index, scope.row)">撤回</el-button>
      </template>
    </el-table-column>
  </el-table>
</template>
<script>
  export default {
    props:['tableData'],
    data() {
      return {
        // tableData: [{
        //   orderId: '1',
        //   from_site_name: '东方高炉北高铁站',
        //   to_site_name: '上海市普陀区金沙江路 1518 弄',
        //   need_permit_name:"ttest",
        //   need_truck_count:"test",
        //   calc_truck_result:"test",
          
        // },]
      }
    }
  }
</script>
<style scoped>
 
</style>