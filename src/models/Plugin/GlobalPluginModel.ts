export type GlobalPluginModel = {
  id: number;
  name: string;
  archived: boolean;
  keys?: GlobalPluginKey[];
};

export type GlobalPluginKey = {
  value: string;
  key: number;
};
