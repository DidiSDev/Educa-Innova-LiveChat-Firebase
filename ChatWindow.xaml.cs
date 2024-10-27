using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Educa_Innova
{
    public partial class ChatWindow : Window
    {
        private string usuario;
        private IFirebaseClient cliente;
        private Dictionary<string, mensajesChat> mensajitos;

        public ChatWindow(string usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
            cliente = Conexion.GetInstance().GetClient();
            cargarMensajes(); //NADA MAS ENTRAR CON EL USUARIO "X"
            //DEJOC ARGADOS TODOS LOS MENSAJES QUE TIENE
        }

        //KEYDOWN PARA ACTIVAR ENVIAR CON INTRO
        private void intro(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //DENTRO DEL KEYDOWN INTRO LLAMO AL BOTON ENVIAR
                btnEnviar_Click(sender, e);
            }
        }

        private async void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para enviar el mensaje
            string mensajeTexto=txtMensajeEnviar.Text;
            if (string.IsNullOrEmpty(mensajeTexto))
            {
                //EL RETURN ES UNA MARAVILLA PORQUE NOS AHORRAMOS UN DO{}WHILE
                MessageBox.Show("Escribe... No se puede enviar el vacío :(");
                return;
            }

            mensajesChat mensajes = new mensajesChat()
            {
                nombreUsuario = this.usuario,
                 mensaje = mensajeTexto,
                fechaMensaje=DateTime.Now
            };
            
            try
            {
                //EL MENSAJE AL IGUAL QUE EN FIRESHARP TENDRÁ UNA ID GENERADA AUTOMATICAMENTE
                string id=Guid.NewGuid().ToString("N");
                //CREAMOS LA CARPETA CHAT EN FIREBASE
                SetResponse respuesta = await cliente.SetAsync($"Chat/{id}", mensajes);

                if (respuesta.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    //LOS MENSAJES QUE APARECE EN EL TEXTBOX TAMBIEN TIENEN QUE TENER LA FECH AY EL MENSAJE
                    txtMensajes.AppendText($"{mensajes.nombreUsuario} ({mensajes.fechaMensaje.ToShortTimeString()}): {mensajes.mensaje}\n");
                    //FORMA DE IR AL ULTIMO MENSAJE ENVIADO SI HAY MUCHISIMOES ESCRITOS:

                    txtMensajes.ScrollToEnd();

                }
                else
                {
                    MessageBox.Show("Error al enviar el mensaje", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("¡Lo lamentamos! Ha habido un error al enviar el mensaje");
            }

            txtMensajeEnviar.Clear();
        }
        //CARGAR MENSAJES
        private async void cargarMensajes()
        {
            try
            {
                FirebaseResponse respuesta = await cliente.GetAsync("Chat");
                if (respuesta.Body != "null")
                {
                    Dictionary<string, mensajesChat> mensajes = respuesta.ResultAs<Dictionary<string, mensajesChat>>();
                    var mensajesOrdenados = mensajes.Values.OrderBy(m => m.fechaMensaje);
                    foreach (var msj in mensajesOrdenados)
                    {
                        txtMensajes.AppendText($"{msj.nombreUsuario} ({msj.fechaMensaje.ToShortTimeString()}): {msj.mensaje}\n");
                        txtMensajes.ScrollToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los mensajes");
            }
        }


        private void btnLimpiarPantalla_Click(object sender, RoutedEventArgs e)
        {
            //EL BOTÓN PARA LIMPIAR PANTALLA QUE APARECE UNA VEZ SE HAN CARGADO LOS MENSAJES
            txtMensajes.Clear();
            btnLimpiarPantalla.Visibility = Visibility.Collapsed;
        }

        private async void cmbUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SOLAMENTE MOSTRARE LOS MENSAJITOS DEL SEÑOR QUE ELIJA EN EL COMBOBOX
            if (cmbUsuarios.SelectedIndex != null)
            {
                string u=cmbUsuarios.SelectedItem.ToString();
                txtMensajes.Clear();
                //SOLO LOS MENSAJES DE "u"
                foreach (var m in mensajitos)
                {
                    if (m.Value.nombreUsuario.Equals(u))
                    {
                        //SOLAMENTE LOS DEL USUARIO RECOGIDO EN COMBO
                        txtMensajes.AppendText($"{m.Value.nombreUsuario} ({m.Value.fechaMensaje}) {m.Value.mensaje}\n");
                        btnLimpiarPantalla.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            //MAS TARDE USARE SAVEFILEDIALOG
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text File(*.txt)|*.txt"; //PERMITIMOS UNICAMENTE ESCRIBIR
            //ARCHIVOS DE TEXTO
            sfd.Title = "Guardar mensajes";
            //LE DOY NOMBRE POR DEFECTO Mensajes_del_chat
            sfd.FileName = "Mensajes_del_chat";
            if (sfd.ShowDialog()==true)
            {
                try
                {
                    //COMO EN EL TEXTBOX YA ESTÁ ORDENADO CON SALTOS DE LÍNEA, SE ESCRIBIRÁ EXACTAMENTE IGUAL A COMO LO VEMOS EN LA PANTALLA, CON 1 LÍNEA DE CÓDIGO SE INSERTARÁ TODO
                    File.WriteAllText(sfd.FileName, txtMensajes.Text);
                    MessageBox.Show("El fichero txt se ha creado correctamente y los mensajes de la ventana se han guardado en él");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error al guardar el fichero txt");
                }
            }
        }

        private async void btnVer_Click(object sender, RoutedEventArgs e)
        {
            //LO PRIMERO ES SACAR TODOS LOS MENSAJES DE FIREBASE DE UN GOLPE
            FirebaseResponse respuesta = await cliente.GetAsync("Chat"); //SACO EL CHAT PORQUE EL CHAT CONTIENE TODO, CONTIENE EL NOMBRE DE USUARIO, LA FECHA Y EL MENSAJE, ASÍ NO TENGO
            //QUE OBTENER DOS RESPUESTA, UNA PARA CHAT Y OTRA PARA USUARIOS
            if (respuesta.Body!="null") //OBTENEMOS DATOS EN STRING
            {
                mensajitos =respuesta.ResultAs<Dictionary<string, mensajesChat>>();
                //LIMPIAMOS COMO BUENA PRÁCTICA
                txtMensajes.Clear();
                cmbUsuarios.Items.Clear(); //LIMPIAMOS ESTO POR SI HABIA ALGO GUARDADO, YA Q EL SIGUIENTE EJERCICIO CONSISTE EN GUARDAR AHÍ LOS DATOS DE ESTE BOTÓN
                List <string> usuariosUnicos=new List<string>();
                var mensajesOrdenados = mensajitos.Values.OrderBy(m => m.fechaMensaje); //SIEMPRE ORDENAMOS POR FECHA
                foreach (var msj in mensajesOrdenados)
                {
                    //NO ENTIENDO POR QUÉ AQUÍ TENGO QUE ACCEDER CON .Value.dato Y ARRIBA NO...
                    txtMensajes.AppendText($"{msj.nombreUsuario} ({msj.fechaMensaje.ToShortTimeString()}): {msj.mensaje}\n");
                    //TAMBIEN DEBO GUARDAR EN EL COMBOBOX EL NOMBRE DEL USUARIO.
                    if (!cmbUsuarios.Items.Contains(msj.nombreUsuario))
                    {
                        //GUARDO SOLO SI NO ESTA EL USUARIO
                        cmbUsuarios.Items.Add(msj.nombreUsuario);
                    }
                }
            }

        }

        private async void btnBuscarFecha_Click(object sender, RoutedEventArgs e)
        {
            DateTime? fechaSeleccionada=datePickerFecha.SelectedDate;
            if (fechaSeleccionada==null)
            {
                MessageBox.Show("Debes seleccionar una fecha para cargar los mensajes ^^");
                return;
            }
            try
            {
                //CARGAMOS MENSAJES DE FIREBASE
                FirebaseResponse respuesta = await cliente.GetAsync("Chat");
                if (respuesta.Body!="null")
                {
                    Dictionary<string, mensajesChat> mensajes=respuesta.ResultAs<Dictionary<string, mensajesChat>>();

                    //UNA VEZ CARGADOS, LOS FILTRAMOS PERO NO SE DEBEN INCLUIR LOS DEL USER ACTIVO
                    //OBLIGATORIO PONER m.fechaMensaje.Date, PORQUE CON fechaMensaje sin el .date RECONOCE SOLAMENTE MENSAJES ENVIADOS A LAS 00:00:00.
                    var mensajesFiltrados = mensajes.Values.Where(m => m.fechaMensaje.Date == fechaSeleccionada.Value.Date && m.nombreUsuario!= this.usuario).OrderBy(m=>m.fechaMensaje);
                    //TAMBIEN QUEDAN ORDENADOS POR ORDEN DE ENVÍO

                    //ANTES DE MOSTRARLOS VAMOS A LIMPIAR LOS QUE HAYA YA CARGADOS EN EL TEXTBOX
                    txtMensajes.Clear();
                    if (!mensajesFiltrados.Any())
                    {
                        txtMensajes.AppendText("NO HAY MENSAJES ESCRITOS EN LA FECHA SELECCIONADA.\n");
                        return ;
                    }

                    foreach (var msj in mensajesFiltrados)
                    {
                        txtMensajes.AppendText($"{msj.nombreUsuario} ({msj.fechaMensaje.ToShortTimeString()}): {msj.mensaje}\n");
                    }

                    txtMensajes.ScrollToEnd();
                }
                else
                {
                    //MUY SIMILAR A QUE NO HAYA "ANY" MENSAJES
                    txtMensajes.AppendText("NO HAY MENSAJES EN LA BASE DE DATOS\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar mensajes de la fecha seleccionada");
            }
        }
    }
}
