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
      path: '/users/:userId', 
      name: 'boxes', component: BoxesView
    },
    {
      path: '/users/:userId/boxes/:boxId', 
      name: 'singlebox', component: SingleBoxView
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/BoxesView.vue')
    }
  ]
})

export default router
