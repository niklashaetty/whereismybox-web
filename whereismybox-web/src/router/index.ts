import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import BoxesView from '../views/BoxesView.vue'
import SingleBoxView from '@/views/SingleBoxView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/collections/:collectionId', 
      name: 'boxes', component: BoxesView
    },
    {
      path: '/collections/:collectionId/boxes/:boxId', 
      name: 'singlebox', component: SingleBoxView
    }
  ]
})

export default router
