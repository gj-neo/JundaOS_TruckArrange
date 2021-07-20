<template>
    
</template>
<script>
// const server_address ="http://localhost:6258/";//本地调试环境
const server_address ="http://114.55.106.80:7200/";//测试环境
//const server_address ="http://114.55.106.80:7000/";//生产环境
//工具类函数
    //创建标准接口的查询函数参数，即query_type,query_condition
    function Create_CommonParam_HttpGet(queryType,queryCondition){
        return  '?query_type='+queryType+'&query_condition='+queryCondition;
    }
    //创建非标准接口的查询函数参数，即自定义的查询类型，默认限制为一个
    function Create_UniqueParam_HttpGet(queryTypeName,queryCondition){
        return  '?'+queryTypeName+'='+queryCondition;
    }
    //根据传入的api生成完整的请求地址（不带参数）
    function GetApiUrl(apiName){
        return server_address+apiName;
    }
export default {
    server_address,
    Create_CommonParam_HttpGet,
    Create_UniqueParam_HttpGet,
    GetApiUrl,
}
</script>