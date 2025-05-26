// Learn more https://docs.expo.io/guides/customizing-metro
const { getDefaultConfig } = require('expo/metro-config');

/** @type {import('expo/metro-config').MetroConfig} */
const config = getDefaultConfig(__dirname);

// Add this to fix react-native-reanimated build issues on Windows
config.resolver.extraNodeModules = {
  'react-native-reanimated': __dirname + '/node_modules/react-native-reanimated',
};

// Add this to handle the worklets properly
config.transformer.getTransformOptions = async () => ({
  transform: {
    experimentalImportSupport: false,
    inlineRequires: true,
  },
});

module.exports = config;
