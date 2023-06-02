import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import BoxesView from '../views/BoxesView.vue'
import SingleBoxView from '@/views/SingleBoxView.vue'
import LoginView from '@/views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
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
