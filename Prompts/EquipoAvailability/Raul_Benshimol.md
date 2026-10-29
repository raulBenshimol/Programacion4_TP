.-Le pase el tp con el enuciado.

.-Vamos ac crear una estructura primeramente es con C# .NET 10.

.-Vamos a trabajar en Visial Studio Comunidad. Vamos a crear todos los microservivios yo le puse AvailabilitySite al primero.

.-Vamos con el siguiente Payment.

.-Antes de las referencias como manejamos el autorition server.

.-Creemoslo.

.-Tenemos la estructura macro ahora vamos un microservicio a vez , no he desarrollado nada solo la estructura sin las relaciones ni nada.

.-Antes de hacer codigo dame la estructura completa del availability completa, no codigo aun solo las clases.

.-Dame primero proyecto mas clase de cada uno.

.-DAME TODO DE NUEVO PERO CON LOS NOMBRES DE LOS ANILLOD DE CLEAN ARCHITECTURE.

.-Codigo no, vamos armar los diagramas y toda la estructura primero con las relaciones y dependencias deja que yo lleve el mando.

.-Codigo aun no.

.-listo el anillo 1

.-vamos a dejarlo asi por ahora despues cuando pasemos al codigo ajustamos si hace falta, no olvides decirme para ajustar el diagrama mas adelante

.-asi vamos, una consulta responde acotado, SolicitudReserva donde lo sacamos y otra pregunta no faltaria algo como SolicitarCompraUseCase?

.-si vamos con el anillo 3 primero solamente

.-nose no me cuadra el nombre delas clasesw del tercer anillo ve lo que tengo analicemos los dos

.-mira vien la foto y como llamamos el MVC

.-te enredaste vamos desde cero con el anillo tres yo ese anillo lo voy a llamar Availability.Controller y tu dame lo de adentro

.-tengo una duda antes de continuar como es la relacion de los objetos del dto con mis clases

.-esperate que te diga si continuamos yo marco el paso ok? aun te estoy preguntando cosas

.-+ SolicitarReserva(solicitudRecuestDto: SolicitudRequestDto): SolicitudResponseDto;   no deberaia queadar asi?

.-vamos con el ultimo

.-a este lo vamos a llamar Availabality.Persistencia

.-antes de continuar no deberia ser asi creo que la ultima capa esta quedando mal

.-creo que esta mal la parte del anillo 4 ya que nuestras clases tienen que interactuar con el controller para el paso de la informacion, osea, este tiene que hacer el puente con el Interactor del anillo 2 y esperar la respuesta que va a salir por el controller al anillo 4

.-y si hacemos el anillo 3 asi?no quedaria mejor ty dejamos otro controleer afuera para la api

.-el registerService que atributos lleva y que implenta

.-esta alucinando para que pusimos el interactor y el presenter enconces

.-eso ultimo esta mal ya con el presenter y el interactor no tengo que hacer comunicacion directa con esas interfaces solo me queda que mande la info por el controller

.-falto poner el controller del anillo 4 ya sera de la api como tal

.- vamos con booking

.- no quiero codigo vamos a diseñar el diagrama de clases

.-es en capas
