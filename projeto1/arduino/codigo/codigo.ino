#include <DHT.h>

const int pino_A0 = A0;  // Define o pino analógico A0
const int ledPin = 13;   // Pino do LED (ajuste conforme necessário)
bool ledEstado = false;  // Estado inicial do LED

#define DHTPIN 4          // Pino do DHT11
#define DHTTYPE DHT11     // Tipo de sensor

DHT dht(DHTPIN, DHTTYPE);

void setup() {
  Serial.begin(9600);         // Inicia comunicação serial
  pinMode(LED_BUILTIN, OUTPUT);  
  pinMode(ledPin, OUTPUT);    // Define pino do LED como saída
  digitalWrite(ledPin, LOW);  // LED começa desligado
  dht.begin();  
}

void loop() {
  if (Serial.available()) {
    String comando = Serial.readStringUntil('\n');
    comando.trim();

    if (comando == "L") {
      digitalWrite(ledPin, HIGH); // Liga o LED
    } else if (comando == "D") {
      digitalWrite(ledPin, LOW);  // Desliga o LED
    }
  }

  float t = dht.readTemperature(); // Lê apenas a temperatura

  if (isnan(t)) {
    Serial.println("Falha na leitura do sensor DHT11!");
    return;
  }

  float leitura_A0 = analogRead(pino_A0);
  float tensao_A0 = leitura_A0 * (5.0 / 1023.0); // Converte leitura em tensão

  // Envia apenas temperatura e tensão
  Serial.print(t);  
  Serial.print(","); 
  Serial.println(tensao_A0, 2);  

  delay(1000);  // Aguarda 1 segundo
}
