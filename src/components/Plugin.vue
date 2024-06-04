<script lang="ts" xmlns:https="http://www.w3.org/1999/xhtml">
  export default {
    name: 'Plugin',
    props: {
      pluginName:{
        type: String,
        required: true
      },
        url:{
          type: String,
          required: true
        }
      }
    }
</script>

<template class="template">
    <div class="pluginContainer">
      <h3 class="pluginName text">{{pluginName}}</h3>
      <p class="url text">{{url}}</p>
    </div>
</template>

<style scoped lang="scss">

  @import url('https://fonts.googleapis.com/css2?family=Manrope:wght@200..800&display=swap');

  .url{
    margin: 0px;
    font-family: 'Manrope', sans-serif;
    font-weight: normal;
    color: #ff002e;
    overflow: hidden; /* Versteckt überfließenden Text */
    text-overflow: ellipsis; /* Fügt ... hinzu, wenn der Text überfließt */
    white-space: nowrap; /* Verhindert Zeilenumbrüche */
    font-size: small;
  }

  .pluginName{
    margin: 0;
    font-family: 'Manrope', sans-serif;
    font-weight: bold;
    color: #0B0D0F;
    font-size: medium;
  }

  .pluginContainer{
    background-color: white;
    box-shadow: rgba(100, 100, 111, 0.2) 0px 7px 29px 0px;
    border-radius: 10px;
    width: 20%;
    height: 10%;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    transition: transform 0.2s ease-in-out;
    &:hover{
      transform: scale(1.05);
    }
    &:active{
      transform: scale(1);
    }
  }

</style>
