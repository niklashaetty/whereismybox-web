import { createRouter, createWebHistory } from 'vue-router'
import RedirectView from '../views/RedirectView.vue'
import RegisterView from '../views/RegisterView.vue'
import CollectionView from '../views/CollectionView.vue'
import DashboardView from '../views/DashboardView.vue'
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
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/collections/:collectionId', 
      name: 'collection', 
      component: CollectionView
    },
    {
      path: '/collections/:collectionId/boxes/:boxId', 
      name: 'singlebox', 
      component: SingleBoxView
    }
  ]
})

export default router
