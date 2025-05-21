import mitt from 'mitt';
// this handles global errors
type Events = {
  criticalAuthFailure: void;
};
export const appEventBus = mitt<Events>();
