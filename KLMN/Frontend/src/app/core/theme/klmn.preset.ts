import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

/** KLMN kurumsal sağlık takip ekranlarının PrimeNG 21 tema preset'i. */
export const KlmnPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#e8f7f6',
      100: '#d1efed',
      200: '#a8ddda',
      300: '#79c9c7',
      400: '#3eafb0',
      500: '#087f81',
      600: '#066c70',
      700: '#075b62',
      800: '#114c54',
      900: '#143d45',
      950: '#102c34'
    },
    colorScheme: {
      light: {
        surface: {
          0: '#ffffff',
          50: '#f7fafb',
          100: '#f2f6f8',
          200: '#e9f0f2',
          300: '#dce6e9',
          400: '#b7c7cf',
          500: '#859aa7',
          600: '#647b88',
          700: '#506677',
          800: '#263a4b',
          900: '#172d3e',
          950: '#0d253a'
        }
      }
    }
  }
});
