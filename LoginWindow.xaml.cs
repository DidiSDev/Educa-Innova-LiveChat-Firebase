using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Educa_Innova
{
    public partial class LoginWindow : Window
    {
      
        private IFirebaseClient client;

        public LoginWindow()
        {
            InitializeComponent();
            // INICIALIZAMOS EL CLIENTE DE FIRESHARP
            client = Conexion.GetInstance().GetClient();
        }
        //BOTÓN REGISTRAR QUE ME LLEVA A LA VENTANA REGISTRO
        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            RegistroWindow registroWindow = new RegistroWindow(this);
            registroWindow.Show();
            this.Hide();
        }

        //MÉTODO PARA EL INTRO, EN CONSTRUCTOR SIEMPRE "KeyEventArgs" PARA RECOGER SU SOURCE
        //EL NOMBRE DE ESTE MÉTODO SE ESTABLECE EN XAML EN LA CARACTERISTICA: KeyDown="pulsarIntro(nombreMétodo)"
        //IMPORTANTÍSIMO PONER ARRIBA EN XAML DENTRO DE Window->  KeyDown="pulsarIntro" que es como se llama aquí el métopdo
        private void pulsarIntro(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                btnEntrar_Click(sender, e);
            }
        }

        // AL PULSAR LOGIN VERIFICAMOS SI ESTÁ O NO EL USUARIO
        private async void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            var usuario = txtUsuario.Text;
            var clave = txtClave.Password;

            // VERIFICAMOS QUE EL USUARIO EXISTA EN LA BASE DE DATOS
            FirebaseResponse respuesta = await client.GetAsync("Usuarios/" + usuario);
            //NO VOY A USAR DICCIONARIOS, LO HAGO COMO EN JAVA
            Usuario usuarioExistente = respuesta.ResultAs<Usuario>(); //ALMACENAMOS RESPUESTA

            if (usuarioExistente != null && usuarioExistente.Clave == clave)
            {
                //ABRIMOS CHAT SI LOS DATOS SON CORRECTOS
                ChatWindow chatWindow = new ChatWindow(usuario);
                chatWindow.Show();
                this.Close();
            }
            else
            {
                // ERROR SI NO
                //LIMPIAMOS
                txtUsuario.Text = "";
                txtClave.Password = "";
                MessageBox.Show("Usuario o contraseña erróneos");
            }
        }
    }

   
}
