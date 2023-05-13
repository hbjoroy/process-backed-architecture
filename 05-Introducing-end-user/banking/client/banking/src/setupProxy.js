const { createProxyMiddleware } = require('http-proxy-middleware');
module.exports = function(app) {
  app.use(
    '/api/payments',
    createProxyMiddleware({
      target: 'http://localhost:5282',
      pathRewrite: {
        '^/api/': '/', // remove base path
      },
    })
  );
};