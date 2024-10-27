using Firebase.Database;
using FireSharp.Config;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Educa_Innova
{
    
    class Conexion
    {
        private static Conexion conexion;
        private IFirebaseClient bbdd;
        private readonly IFirebaseConfig config;
        private Conexion()
        {
            config = new FirebaseConfig
            {
                AuthSecret = "rp7QF4G6pZdekOQ3TIw1K0ahlfuZR6Ditq2aM6GE",
                BasePath = "https://educa-innova-default-rtdb.europe-west1.firebasedatabase.app/"
            }; 
            bbdd=new FireSharp.FirebaseClient(config);
            if (bbdd==null)
            {
                MessageBox.Show("Error al establecer la conexión con la base de datos");
            }

        }
        public static Conexion GetInstance()
        {
            if (conexion==null)
            {
                conexion = new Conexion();
            }
            return conexion;
        }

        public IFirebaseClient GetClient()
        {
            return bbdd;
        }
    }
    
    
}
