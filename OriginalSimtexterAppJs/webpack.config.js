const path = require('path');

module.exports = {
    // Definisci l'entry point principale
    entry: './src/app2.js',

    // Definisci la destinazione del bundle generato
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'bundle.js'
    },

    // Configura il server di sviluppo (opzionale)
    devServer: {
        static: {
            directory: path.join(__dirname, 'dist'),
        },
        port: 8080,
        open: true
    },

    // Modalità di build: 'development' o 'production'
    mode: 'development',

    // Configurazione dei loader (es. per ES6, CSS, ecc.)
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader', // Se usi Babel per transpiling
                    options: {
                        presets: ['@babel/preset-env']
                    }
                }
            },
            // Aggiungi altri loader qui se necessario
        ]
    },

    // Configurazione dei plugin (es. per HTML)
    plugins: [
        // Puoi aggiungere HTMLWebpackPlugin per generare l'HTML
    ]
};