<script setup lang="ts">
import router from '@/router';
import Header from '@/components/Header.vue'
import ImageCard from '@/components/ImageCard.vue'

</script>
<template>
  <Header />
  <div>
    <div class="logincontainer">

      <!-- Github -->
      <a href="/.auth/login/github?post_login_redirect_uri=/">
      <div class="loginsection">
        <div class="logo"> 
          <img src="../assets/github-logo.svg" width="50px" height="50px"> 
        </div>
        <div class="text" >
          <p class="logintext" > Sign in with Github</p>
        </div>
      </div>
    </a>

    <!-- Google -->
    <a href="/.auth/login/google?post_login_redirect_uri=/">
      <div class="loginsection">
        <div class="logo"> 
          <img src="../assets/google-logo-normal.svg" width="60px" height="60px"> 
        </div>
        <div class="text" >
          <p class="logintext" > Sign in with Google</p>
        </div>
      </div>
    </a>
  </div>
  </div>
  
</template>

<style scoped lang="scss">
.logincontainer {
  margin-top: 10px;
  width: 300px;

  padding: 10px;
  border-radius: 3px;
  align-items: center;
  user-select: none; /* Non-prefixed version, currently */
  -webkit-user-select: none;
  display: block;
  margin-left: auto;
  margin-right: auto;
}

.loginsection{
  margin-bottom: 5px;
  height: 80px;
  border-radius: 3px;
  display: flex;
  text-align: center;
  align-items: center;
  cursor: pointer;
  background-color: #f7faf8;
  color: black;

  box-shadow:
  0 0.7px 0.5px rgba(0, 0, 0, 0.034),
  0 1.5px 1.7px rgba(0, 0, 0, 0.048),
  0 3.5px 2.5px rgba(0, 0, 0, 0.06),
  0 4.3px 4.9px rgba(0, 0, 0, 0.072),
  0 10.8px 8.4px rgba(0, 0, 0, 0.086);
}
.logo{
  width: 100px;
}

.loginbutton{
  margin-top: auto;
  text-align: center;
  display: flex;
  justify-content: center;
}

.info-cards {
  margin-left: auto;
  margin-right: auto;
  justify-content: space-evenly;
  @media (min-width: 500px) {
    display:flex;
    width: 900px;
    justify-content: space-evenly;
  }
}

</style>