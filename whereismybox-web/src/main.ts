import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import PrimeVue from 'primevue/config';
import Card from 'primevue/card';
import ToastService from 'primevue/toastservice';
import ConfirmationService from 'primevue/confirmationservice';
import DialogService from 'primevue/dialogservice';
import Tooltip from 'primevue/tooltip';
import { createPinia } from 'pinia'
import Paperizer from 'paperizer'

import './assets/main.css'

import 'primevue/resources/themes/saga-blue/theme.css'       //theme
import 'primevue/resources/primevue.min.css'                 //core css
import 'primeicons/primeicons.css'                           //icons
import { library } from '@fortawesome/fontawesome-svg-core'
import mitt from 'mitt'; 

/* import font awesome icon component */
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { faUserSecret } from '@fortawesome/free-solid-svg-icons'

/* add icons to the library */
library.add(faUserSecret)

const pinia = createPinia()
const app = createApp(App)

app.use(router)
app.use(PrimeVue);
app.use(ToastService);
app.use(ConfirmationService);
app.use(pinia)
app.use(DialogService);
app.use(Paperizer);

app.directive('tooltip', Tooltip);

const emitter = mitt(); 

app.provide('emitter', emitter);       

app.component('Card', Card);
app.component('font-awesome-icon', FontAwesomeIcon)
app.mount('#app')