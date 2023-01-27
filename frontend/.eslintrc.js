module.exports = {
  root: true,
  env: {
    es2021: true,
    'jest/globals': true,
  },
  plugins: [
    'jest',
  ],
  parserOptions: {
    ecmaVersion: '2020',
  },
  settings: {
    'import/resolver': {
      alias: {
        map: [
          ['@', './src'],
        ],
        extensions: ['.js', '.vue'],
      },
    },
  },
  extends: [
    'plugin:vue/essential',
    'plugin:vue/strongly-recommended',
    'plugin:vue/recommended',
    'eslint:recommended',
    'airbnb-base',
  ],
  rules: {
    //
    // Vue Plugin
    //
    'vue/multi-word-component-names': 'off',
    'vue/valid-v-slot': ['error', {
      allowModifiers: true,
    }],
    'vue/max-attributes-per-line': ['error', {
      singleline: { max: 3 },
      multiline: { max: 1 },
    }],
    'no-param-reassign': [
      'error',
      {
        props: true,
        ignorePropertyModificationsFor: [
          'state',
        ],
      },
    ],
  },
};
