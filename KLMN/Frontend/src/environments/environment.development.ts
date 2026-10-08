/**
 * Development ortam ayarlarını içerir. Angular CLI proxy kullanılmaz; API'ye doğrudan HTTPS isteği gönderilir.
 */
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7145/api'
};
