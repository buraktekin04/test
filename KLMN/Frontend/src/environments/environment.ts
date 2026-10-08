/**
 * Production ortam ayarlarını içerir. Frontend ve API aynı origin üzerinden sunulursa /api kullanılır; farklı domain için tam HTTPS API adresini verin.
 */
export const environment = {
  production: true,
  apiBaseUrl: '/api'
};
