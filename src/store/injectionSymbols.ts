import { ProjectsStore } from './ProjectsStore';
import { ProjectInformationStore } from './ProjectInformationStore';
import type { InjectionKey } from 'vue';

const projectsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof ProjectsStore>
>;

const projectInformationStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof ProjectInformationStore>
>;

export { projectsStoreSymbol, projectInformationStoreSymbol };
