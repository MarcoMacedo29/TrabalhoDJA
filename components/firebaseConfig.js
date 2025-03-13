// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import { getFirestore } from "firebase/firestore"; // Importação do Firestore

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyBgRmEXRUxtA7uroqlICzyFxqoFcTJm6Cw",
  authDomain: "webjogo-f4a87.firebaseapp.com",
  projectId: "webjogo-f4a87",
  storageBucket: "webjogo-f4a87.firebasestorage.app",
  messagingSenderId: "99126640807",
  appId: "1:99126640807:web:db672cd196bca41299db82",
  measurementId: "G-CEVHD2GHGR",
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

// Initialize Analytics (opcional)
const analytics = getAnalytics(app);

// Initialize Firestore
const db = getFirestore(app); // Inicializando o Firestore

// Exportando o Firestore para uso nos outros componentes
export { db };
