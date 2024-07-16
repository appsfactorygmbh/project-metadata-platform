import { beforeEach, describe, expect, it } from 'vitest';
import { useProjectEditStore } from '../ProjectEditStore';
import { createPinia, setActivePinia } from 'pinia';
import type { PluginEditModel, PluginModel } from '../../../models/Plugin';

describe('ProjectEditStore', () => {
  let store: ReturnType<typeof useProjectEditStore>;

  beforeEach(() => {
    setActivePinia(createPinia());
    store = useProjectEditStore();
  });

  it('initializes with default state', () => {
    expect(store.pluginChanges.size).toBe(0);
    expect(store.projectInformationChanges).toEqual([]);
    expect(store.canBeCreated).toBe(true);
    expect(store.pluginsWithUrlConflicts).toEqual([]);
    expect(store.duplicatedUrls.size).toBe(0);
    expect(store.emptyUrlFields.size).toBe(0);
    expect(store.emptyDisplaynameFields.size).toBe(0);
  });

  it('adds and removes empty fields correctly', () => {
    store.addEmptyUrlField(1);
    expect(store.emptyUrlFields.size).toBe(1);
    expect(store.emptyUrlFields.has(1)).toBe(true);

    store.removeEmptyUrlField(1);
    expect(store.emptyUrlFields.size).toBe(0);
    expect(store.emptyUrlFields.has(1)).toBe(false);

    store.addEmptyDisplaynameField(1);
    expect(store.emptyDisplaynameFields.size).toBe(1);
    expect(store.emptyDisplaynameFields.has(1)).toBe(true);

    store.removeEmptyDisplaynameField(1);
    expect(store.emptyDisplaynameFields.size).toBe(0);
    expect(store.emptyDisplaynameFields.has(1)).toBe(false);
  });

  it('resets changes correctly', () => {
    store.addEmptyUrlField(1);
    store.addEmptyDisplaynameField(1);
    store.resetChanges();
    expect(store.pluginChanges.size).toBe(0);
    expect(store.projectInformationChanges).toEqual([]);
    expect(store.canBeCreated).toBe(true);
    expect(store.pluginsWithUrlConflicts).toEqual([]);
    expect(store.duplicatedUrls.size).toBe(0);
    expect(store.emptyUrlFields.size).toBe(0);
    expect(store.emptyDisplaynameFields.size).toBe(0);
  });

  it('checks for URL conflicts correctly', () => {
    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      pluginName: 'Test Plugin',
      displayName: 'Test Plugin',
    };
    store.initialAdd(plugin);
    store.initialAdd(plugin);
    store.checkForConflicts();

    expect(store.duplicatedUrls.size).toBe(1);
    expect(store.duplicatedUrls.get('http://example.com')?.length).toBe(2);
    expect(store.getPluginsWithUrlConflicts.length).toBe(2);
  });

  it('adds and updates plugins correctly', () => {
    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    const pluginEdit: PluginEditModel = {
      ...plugin,
      editKey: 0,
      isDeleted: false,
    };

    const index = store.initialAdd(plugin);
    expect(store.pluginChanges.size).toBe(1);
    expect(store.pluginChanges.get(index)).toEqual(pluginEdit);

    const updatedPluginEdit: PluginEditModel = {
      ...pluginEdit,
      displayName: 'Updated Plugin',
      pluginName: 'Test Plugin',
    };
    store.updatePluginChanges(index, updatedPluginEdit);
    expect(store.pluginChanges.get(index)).toEqual(updatedPluginEdit);
  });

  it('deletes plugins correctly', () => {
    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    const index = store.initialAdd(plugin);

    store.deletePlugin(index);
    expect(store.pluginChanges.get(index)?.isDeleted).toBe(true);
  });

  it('computes getters correctly', () => {
    expect(store.getProjectInformationChanges).toEqual([]);

    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    const index = store.initialAdd(plugin);

    expect(store.getPluginChanges.length).toBe(1);

    store.deletePlugin(index);
    expect(store.getPluginChanges.length).toBe(0);
  });

  it('computes canBeAdded correctly', () => {
    expect(store.getCanBeAdded).toBe(true);

    store.addEmptyUrlField(1);
    expect(store.getCanBeAdded).toBe(false);

    store.removeEmptyUrlField(1);
    expect(store.getCanBeAdded).toBe(true);

    store.addEmptyDisplaynameField(1);
    expect(store.getCanBeAdded).toBe(false);

    store.removeEmptyDisplaynameField(1);
    expect(store.getCanBeAdded).toBe(true);

    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    store.initialAdd(plugin);
    store.initialAdd(plugin); // Adding the same plugin to simulate a conflict
    store.checkForConflicts();

    expect(store.getCanBeAdded).toBe(false);
  });
  // Add test to check for updating project information
  it('updates project information correctly', () => {
    const project = {
      id: 1,
      projectName: 'Test Project',
      clientName: 'Test Client',
      businessUnit: 'Test Business Unit',
      teamNumber: 1,
    };
    store.updateProjectInformationChanges(project);
    expect(store.getProjectInformationChanges).toEqual(project);
  });

  // write tests for the getCanBeAdded getter
});
