import { createRouter, createWebHistory } from 'vue-router'
import RedirectView from '../views/RedirectView.vue'
import RegisterView from '../views/RegisterView.vue'
import BoxesView from '../views/BoxesView.vue'
import SingleBoxView from '@/views/SingleBoxView.vue'
import LoginView from '@/views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'redirect',
      component: RedirectView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/collections/:collectionId', 
      name: 'boxes', 
      component: BoxesView
    },
    {
      path: '/collections/:collectionId/boxes/:boxId', 
      name: 'singlebox', 
      component: SingleBoxView
    }
  ]
})

export default router
