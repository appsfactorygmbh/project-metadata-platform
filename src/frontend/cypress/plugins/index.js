// From: https://www.cypress.io/blog/generate-high-resolution-videos-and-screenshots
on('before:browser:launch', (browser = {}, launchOptions) => {
  console.log(
    'launching browser %s is headless? %s',
    browser.name,
    browser.isHeadless,
  );

  // the browser width and height we want to get
  // our screenshots and videos will be of that resolution
  const width = 1920;
  const height = 1200;

  console.log('setting the browser window size to %d x %d', width, height);

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
