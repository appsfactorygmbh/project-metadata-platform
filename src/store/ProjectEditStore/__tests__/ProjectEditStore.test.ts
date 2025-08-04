import { beforeEach, describe, expect, it } from 'vitest';
import { useProjectEditStore } from '../ProjectEditStore';
import { createPinia, setActivePinia } from 'pinia';
import type { PluginEditModel, PluginModel } from '../../../models/Plugin';
import type { DetailedProjectModel } from '@/models/Project';

describe('ProjectEditStore', () => {
  let store: ReturnType<typeof useProjectEditStore>;

  beforeEach(() => {
    setActivePinia(createPinia());
    store = useProjectEditStore();
  });

  it('initializes with default state', () => {
    expect(store.pluginChanges.size).toBe(0);
    expect(store.projectInformationChanges).toEqual({
      projectName: '',
      clientName: '',
      teamId: null,
      offerId: '',
      company: '',
      ismsLevel: 'NORMAL',
      companyState: 'EXTERNAL',
    });
    expect(store.canBeCreated).toBe(true);
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

  it('resets plugin changes correctly', () => {
    store.addEmptyUrlField(1);
    store.addEmptyDisplaynameField(1);
    store.resetPluginChanges();
    expect(store.pluginChanges.size).toBe(0);
    expect(store.canBeCreated).toBe(true);
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
    expect(store.getProjectInformationChanges).toEqual({
      projectName: '',
      clientName: '',
      teamId: null,
      offerId: '',
      company: '',
      ismsLevel: 'NORMAL',
      companyState: 'EXTERNAL',
    });

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
    store.setProjectInformation({
      id: 1,
      projectName: '',
      clientName: '',
      slug: '',
      isArchived: false,
      offerId: 'Offer1',
      company: 'Test Firma',
      ismsLevel: 'VERY_HIGH',
      companyState: 'EXTERNAL',
      team: undefined,
    });

    expect(store.getCanBeAdded).toBe(false);

    store.updateProjectInformationChanges({
      projectName: 'Test Project',
      clientName: 'Test Client',
      offerId: 'Offer1',
      company: 'Test Firma',
      ismsLevel: 'VERY_HIGH',
      companyState: 'EXTERNAL',
    });

    store.setProjectInformation({
      id: 1,
      projectName: 'Test Project',
      slug: 'test-project',
      clientName: 'Test Client',
      team: undefined,
      isArchived: false,
      offerId: 'Offer1',
      company: 'Test Firma',
      ismsLevel: 'VERY_HIGH',
      companyState: 'EXTERNAL',
    });

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
    const project: DetailedProjectModel = {
      id: 1,
      projectName: 'Test Project',
      clientName: 'Test Client',
      team: undefined,
      isArchived: false,
      offerId: 'Offer1',
      company: 'Test Firma',
      ismsLevel: 'VERY_HIGH' as 'VERY_HIGH' | 'NORMAL' | 'HIGH',
      companyState: 'EXTERNAL' as 'EXTERNAL' | 'INTERNAL',
      slug: 'test_project',
    };
    store.updateProjectInformationChanges(project);
    expect(store.getProjectInformationChanges).toEqual(project);
  });
  it('sets project information correctly', () => {
    const store = useProjectEditStore();
    const projectInfo: DetailedProjectModel = {
      id: 1,
      projectName: 'New Project',
      slug: 'new-project',
      clientName: 'New Client',
      team: undefined,
      isArchived: false,
      offerId: 'Offer1',
      company: 'New Firma',
      ismsLevel: 'VERY_HIGH' as 'VERY_HIGH' | 'NORMAL' | 'HIGH',
      companyState: 'EXTERNAL' as 'EXTERNAL' | 'INTERNAL',
    };
    store.setProjectInformation(projectInfo);
    expect(store.projectInformationChanges).toEqual(projectInfo);
    expect(store.emptyProjectInformationFields.size).toBe(0);
  });

  it('updates project information changes correctly', () => {
    const store = useProjectEditStore();
    const updatedProjectInfo: DetailedProjectModel = {
      id: 1,
      projectName: 'Updated Project',
      clientName: 'Updated Client',
      team: undefined,
      isArchived: false,
      offerId: 'Offer1',
      company: 'Test Firma',
      ismsLevel: 'VERY_HIGH' as 'VERY_HIGH' | 'NORMAL' | 'HIGH',
      companyState: 'EXTERNAL' as 'EXTERNAL' | 'INTERNAL',
      slug: 'updated_project',
    };
    store.updateProjectInformationChanges(updatedProjectInfo);
    expect(store.projectInformationChanges).toEqual(updatedProjectInfo);
  });

  it('adds empty project information field correctly', () => {
    const store = useProjectEditStore();
    store.addEmptyProjectInformationField('projectName');
    expect(store.emptyProjectInformationFields.has('projectName')).toBe(true);
  });

  it('removes empty project information field correctly', () => {
    const store = useProjectEditStore();
    store.addEmptyProjectInformationField('projectName');
    store.removeEmptyProjectInformationField('projectName');
    expect(store.emptyProjectInformationFields.has('projectName')).toBe(false);
  });

  it('adds new plugin correctly', () => {
    const plugin: PluginEditModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
      editKey: 0,
      isDeleted: false,
    };

    store.addNewPlugin(plugin);
    expect(store.getAddedPlugins.length).toBe(1);
    expect(store.getAddedPlugins[0]).toEqual(plugin);
  });

  it('returns added plugins correctly', () => {
    const plugin1: PluginEditModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin 1',
      pluginName: 'Test Plugin 1',
      editKey: 0,
      isDeleted: false,
    };

    const plugin2: PluginEditModel = {
      id: 2,
      url: 'http://example.com',
      displayName: 'Test Plugin 2',
      pluginName: 'Test Plugin 2',
      editKey: 1,
      isDeleted: false,
    };

    store.addNewPlugin(plugin1);
    store.addNewPlugin(plugin2);
    expect(store.getAddedPlugins.length).toBe(2);
    expect(store.getAddedPlugins).toEqual([plugin1, plugin2]);
  });
});
