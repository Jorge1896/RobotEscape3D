# HELL PROTOCOL

Videojuego de acción y supervivencia 3D desarrollado en Unity 6.5 (6000.5.10f1), para la Evaluación Sumativa 3 de Desarrollo de Videojuegos (TILE22).

## Historia
El reactor del sector 07 está colapsando. Una unidad de combate debe recuperar cinco núcleos de energía y alcanzar el portal de evacuación antes de que se agote el tiempo, evitando las máquinas de seguridad hostiles.

## Cómo ejecutar
1. Clonar el repositorio y seleccionar la rama que contiene la entrega.
2. Abrir la carpeta del proyecto desde Unity Hub con Unity 6000.5.10f1.
3. Esperar la importación de los paquetes y recursos.
4. Abrir Assets/Scenes/RobotEscape.unity y pulsar Play.
5. Presionar Enter para iniciar la misión.

La escena debe incluir los recursos generados y las referencias a los archivos de música. No es necesario ejecutar los generadores de editor si se ha guardado la escena completa.

## Controles
| Control | Acción |
|---|---|
| WASD | Movimiento |
| Mouse | Apuntado al disparar |
| Clic izquierdo | Disparo láser |
| Espacio | Dash |
| Enter | Iniciar misión |
| ESC | Pausar o continuar |
| R | Reiniciar desde pausa o resultado |

## Reglas
- Vida inicial: 100 HP.
- Tiempo: 120 segundos.
- Recuperar cinco núcleos y alcanzar el portal activado para ganar.
- Perder toda la vida o agotar el tiempo produce la derrota.
- Los enemigos tienen 50 HP y reciben 25 de daño por disparo.
- Las baterías recuperan hasta 25 HP, sin superar 100.
- Dash con recarga de 1,25 segundos, sin invulnerabilidad.

## Implementación
| Sistema | Scripts y conceptos |
|---|---|
| Detección | RaycastWeapon: Physics.Raycast, primer impacto, LineRenderer |
| Física 3D | RobotController y EnemyController: Rigidbody, Collider y FixedUpdate |
| Recolección | EnergyCore: OnTriggerEnter; HealthBattery: OnTriggerStay |
| Interfaz | GameManager: Canvas, Text, Image, temporizador y estados |
| Audio | AmbientMusic y GameManager: AudioSource, pistas y efectos |
| Personaje | RobotController, RobotAnimation, CameraController |
| Nivel | GameManager y ExitPortal: condiciones de victoria y derrota |
| Efectos | VisualEffects: ParticleSystem |

## Pruebas manuales
Comprobar movimiento, bloqueo por muros, disparos y daño, recolección, portal bloqueado y activado, victoria, derrota por tiempo y por vida, reinicio, pausa, música, dash y baterías. Las pruebas de desarrollo fueron realizadas en el editor; verificar también una compilación de Windows antes de entregar.

## Créditos
Proyecto académico. Referencias artísticas: DOOM (2016) y DOOM Eternal; no se utilizan recursos originales de esos juegos. Portada generada con ChatGPT y banda sonora creada con Suno. Verificar los permisos de distribución de las pistas según el plan utilizado antes de publicar. Completar los nombres de ambos integrantes y sus contribuciones reales antes de entregar.
