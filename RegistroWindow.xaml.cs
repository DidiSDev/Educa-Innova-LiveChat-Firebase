using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Windows;
using System.Windows.Input;

namespace Educa_Innova
{
    public partial class RegistroWindow : Window
    {
        private IFirebaseClient client;
        private LoginWindow login;

        public RegistroWindow(LoginWindow login)
        {
            this.login = login;
            InitializeComponent();
            client = Conexion.GetInstance().GetClient();
        }

        //METODO PARA EL INTRO COMBINADO CON REGISTRAR
        public void pulsarIntro(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Enter)
            {
                btnRegistrar_Click(sender, e);
            }
        }
        // VOLVER
        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); //CERRAMOS ESTA VENTANA PORQUE LA OTRA HA QUEDADO ABIERTA
            login.Show();
        }
        public void limpiarCampos()
        {
            txtNuevoUsuario.Text = "";
            txtNuevaClave.Password = "";
            txtRepetirClave.Password = "";
        }
        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            var nuevoUsuario = txtNuevoUsuario.Text;
            var nuevaClave = txtNuevaClave.Password;
            var repetirClave = txtRepetirClave.Password;

            //VALIDAR QUE NO HAYA CAMPOS VACÍOS
            if (string.IsNullOrEmpty(nuevoUsuario) || string.IsNullOrEmpty(nuevaClave) || string.IsNullOrEmpty(repetirClave))
            {
                MessageBox.Show("Por favor, rellena todos los campos.");
                limpiarCampos();
                return;
            }

            //VALIDAR QUE LAS CONTRASEÑAS COINCIDEN
            if (nuevaClave != repetirClave)
            {
                MessageBox.Show("Las contraseñas no coinciden.");
                limpiarCampos();
                return;
            }
           


            // VEMOS SI EL USUARIO YA EXISTE EN LA BASE DE DATOS
            FirebaseResponse respuestaBBDD = await client.GetAsync("Usuarios/" + nuevoUsuario);
            Usuario usuarioExistente = respuestaBBDD.ResultAs<Usuario>();

            if (usuarioExistente != null)
            {
                // SI EL USUARIO YA EXISTE, MOSTRAMOS UN MENSAJE DE ERROR
                MessageBox.Show("Ese nombre de usuario ya existe.");
                limpiarCampos();
                return;
            }

            // CREAR OBJETO USUARIO SI NO EXISTE YA
            Usuario nuevo = new Usuario
            {
                Nombre = nuevoUsuario,
                Clave = nuevaClave
            };

            // REGISTRAR USUARIO EN LA BASE DE DATOS
            SetResponse RESPUESTA = await client.SetAsync("Usuarios/" + nuevoUsuario, nuevo);
            if (RESPUESTA.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Usuario registrado correctamente.");
                login.Show(); //VOLVEMOS AL LOGIN TRAS REGISTRO
                this.Close(); //CERRAMOS VENTANA DE REGISTRO
            }
            else
            {
                limpiarCampos();
                MessageBox.Show("Error al registrar el usuario.");
            }
        }
    }
}
