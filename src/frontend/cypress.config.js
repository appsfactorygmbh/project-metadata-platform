import { defineConfig } from 'cypress';
import fs from 'node:fs';
import path from 'node:path';

export default defineConfig({
  e2e: {
    supportFile: false,
    baseUrl: 'http://127.0.0.1:8080',
    screenshotsFolder: '../../screenshots',
    setupNodeEvents(on, config) {
      on('after:screenshot', (details) => {
        if (details.name) {
          const newPath = path.join(
            config.screenshotsFolder,
            `${details.name}.png`,
          );

          return new Promise((resolve, reject) => {
            fs.rename(details.path, newPath, (err) => {
              if (err) return reject(err);
              const specFolder = path.dirname(details.path);
              fs.rmdir(specFolder, () => {});
              resolve({ path: newPath });
            });
          });
        }
      });
      on('before:browser:launch', (browser = {}, launchOptions) => {
        console.log(
          'launching browser %s is headless? %s',
          browser.name,
          browser.isHeadless,
        );
        const width = 1920;
        const height = 1200;

        console.log(
          'setting the browser window size to %d x %d',
          width,
          height,
        );

        if (browser.name === 'chrome' && browser.isHeadless) {
          launchOptions.args.push(`--window-size=${width},${height}`);

          launchOptions.args.push('--force-device-scale-factor=1');
        }

        if (browser.name === 'electron' && browser.isHeadless) {
          launchOptions.preferences.width = width;
          launchOptions.preferences.height = height;
        }

        if (browser.name === 'firefox' && browser.isHeadless) {
          launchOptions.args.push(`--width=${width}`);
          launchOptions.args.push(`--height=${height}`);
          launchOptions.preferences['layout.css.devPixelsPerPx'] = '1.0';
        }

        return launchOptions;
      });
    },
  },
});
