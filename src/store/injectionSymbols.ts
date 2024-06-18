import { ProjectsStore } from './ProjectsStore';

const projectsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof ProjectsStore>
>;

export { projectsStoreSymbol };
