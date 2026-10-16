import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

/** Nizamlı kurumsal KLMN ekranlarının mat çelik mavisi PrimeNG paletidir. */
export const KlmnPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#eef2f5', 100: '#e0e7ed', 200: '#c9d5df',
      300: '#aabdcf', 400: '#7895ab', 500: '#405c74',
      600: '#344c63', 700: '#2a4054', 800: '#26394b',
      900: '#202f3f', 950: '#192635'
    },
    colorScheme: {
      light: {
        surface: {
          0: '#ffffff', 50: '#f7f8fa', 100: '#edf1f4',
          200: '#e1e5ea', 300: '#d3dae1', 400: '#a9b6c1',
          500: '#85909c', 600: '#647281', 700: '#465462',
          800: '#2d3843', 900: '#1c2833', 950: '#202a34'
        }
      }
    }
  }
});
