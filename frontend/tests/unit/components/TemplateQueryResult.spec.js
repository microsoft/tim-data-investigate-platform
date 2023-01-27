/* eslint-disable no-shadow */
import Vue from 'vue';
import Vuetify from 'vuetify';
import { createLocalVue, mount } from '@vue/test-utils';
import TemplateQueryResult from '@/components/TemplateQueryResult.vue';
import { QueryTemplate } from '@/helpers/kustoQueries';

Vue.use(Vuetify);

describe('TemplateQueryResult', () => {
  const localVue = createLocalVue();
  let vuetify;

  beforeEach(() => {
    vuetify = new Vuetify();
  });

  const template = new QueryTemplate({
    menu: 'test menu',
    summary: 'test summary',
    path: 'test path',
    cluster: 'test cluster',
    params: {
      param1: {
        default: 'default-1',
      },
      param2: {
        default: 'default-2',
        type: 'string',
      },
      param3: {
        default: true,
        type: 'boolean',
      },
      param4: {
        default: [],
        type: 'array',
        values: [
          'value-4-1',
          'value-4-2',
        ],
      },
    },
    field: {
      field1: {
        type: 'string',
      },
    },
  });

  const inParams = () => ({
    param1: 'param-1',
    param2: 'param-2',
    param3: false,
    param4: ['value-4-1'],
  });

  const mountFunction = (options) => mount(TemplateQueryResult, {
    localVue,
    vuetify,
    ...options,
  });

  it('renders param of default no type as text field', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-text-field' }).filter((w) => w.text() === 'param1');
    expect(w.exists()).toBeTruthy();
  });

  it('renders param of type string as text field', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-text-field' }).filter((w) => w.text() === 'param2');
    expect(w.exists()).toBeTruthy();
  });

  it('renders param of type boolean as switch', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-switch' }).filter((w) => w.text() === 'param3');
    expect(w.exists()).toBeTruthy();
  });

  it('renders param of type array as select', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-select' }).filter((w) => w.find('label').text() === 'param4');
    expect(w.exists()).toBeTruthy();
  });

  it('renders in param value in text field', () => {
    const wrapper = mountFunction({
      propsData: { inParams: { ...inParams(), param1: 'test-1' }, queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-text-field' }).filter((w) => w.text() === 'param1');
    expect(w.exists()).toBeTruthy();
    expect(w.at(0).vm.internalValue).toEqual('test-1');
  });

  it('renders in param value in switch field', () => {
    const wrapper = mountFunction({
      propsData: { inParams: { ...inParams(), param3: true }, queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-switch' }).filter((w) => w.text() === 'param3');
    expect(w.exists()).toBeTruthy();
    expect(w.at(0).vm.internalValue).toBeTruthy();
  });

  it('renders in param value in select field', () => {
    const wrapper = mountFunction({
      propsData: { inParams: { ...inParams(), param4: ['test-4-1'] }, queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-select' }).filter((w) => w.find('label').text() === 'param4');
    expect(w.exists()).toBeTruthy();
    expect(w.at(0).vm.internalValue).toEqual(['test-4-1']);
  });

  it('updates the param value on text change', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-text-field' }).filter((w) => w.text() === 'param1');
    expect(w.exists()).toBeTruthy();
    w.at(0).find('input').setValue('test-1');
    expect(wrapper.vm.params.param1).toEqual('test-1');
  });

  it('updates the param value on switch change', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-switch' }).filter((w) => w.text() === 'param3');
    expect(w.exists()).toBeTruthy();
    w.at(0).find('input').trigger('click');
    expect(wrapper.vm.params.param3).toBeTruthy();
    w.at(0).find('input').trigger('click');
    expect(wrapper.vm.params.param3).toBeFalsy();
  });

  it('updates the param value on select change', () => {
    const wrapper = mountFunction({
      propsData: { inParams: inParams(), queryTemplate: template },
    });

    const w = wrapper.findAllComponents({ name: 'v-select' }).filter((w) => w.find('label').text() === 'param4');
    expect(w.exists()).toBeTruthy();
    w.at(0).vm.selectItem('test-1');
    expect(wrapper.vm.params.param4).toEqual(['value-4-1', 'test-1']);
  });
});
