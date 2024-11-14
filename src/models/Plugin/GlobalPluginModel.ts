export type GlobalPluginModel = {
  id: number;
  name: string;
  isArchived: boolean;
  keys?: GlobalPluginKey[];
};

export type GlobalPluginKey = {
  value: string;
  key: number;
  archived: boolean;
};
