import Vue from 'vue'
import Router from 'vue-router'
import page_auto_schedule from '../pages/page_auto_schedule.vue'
import page_manual_schedule from '../pages/page_manual_schedule.vue'
import page_permit from '../pages/page_permit'
import page_site from '../pages/page_site'
import page_truck from '../pages/page_truck'
import page_statistic_export from '../pages/page_statistic_export'
import page_refresh from '../pages/page_refresh'
import page_debug from '../pages/page_debug'
import page_statistic_task_record from '../pages/page_statistic_task_record'
import page_statistic_single_truck_attend from '../pages/page_statistic_single_truck_attend'
import page_statistic_total_attend from '../pages/page_statistic_total_attend'
Vue.use(Router)

export default new Router({
  routes: [
    {
      name:'refresh',
      path:'refresh',
      component:page_refresh,
  }
    ,
    {
      path: '/1-1',
      name: 'auto_schedule',
      component: page_auto_schedule,
      meta:{
        keepAlive:true,
        isNeedRefresh:true
      }
    },
    {
      path: '/1-2',
      name: 'manual_schedule',
      component: page_manual_schedule,
      meta:{
        keepAlive:true,
        isNeedRefresh:true
      }
    },
    
    {
      path: '/4',
      name: 'permit',
      component: page_permit
    }
    ,
    {
      path: '/3',
      name: 'site',
      component: page_site
    },
    {
      path: '/2',
      name: 'truck',
      component: page_truck
    },
    {
      path: '/5-3',
      name: 'page_statistic_single_truck_attend',
      component: page_statistic_single_truck_attend
    },
   
    {
      path: '/5-2',
      name: 'page_statistic_task_record',
      component: page_statistic_task_record
    },
    {
      path: '/5-4',
      name: 'page_debug',
      component: page_debug
    }
    ,
    //车辆总出勤
    {
      path: '/5-5',
      name: 'page_statistic_total_attend',
      component: page_statistic_total_attend
    }
  ]  
})
