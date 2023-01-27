<template>
  <v-container class="fill-height" fluid>
    <v-row>
      <v-col class="col-md-8 offset-md-2 text-center">
        <v-container>
          <p class="text-h4">
            Welcome to TIM
          </p>
          <p class="subtitle-1">
            The triage and investigation experience.
          </p>
          <NewQueryButton> Get Started </NewQueryButton>
        </v-container>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import NewQueryButton from '@/components/NewQueryButton.vue';

export default {
  name: 'Welcome',
  components: {
    NewQueryButton,
  },
};
</script>

<style></style>
