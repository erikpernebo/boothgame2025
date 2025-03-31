#define NUM_BUTTONS 5
#define NUM_OUTPUTS 4

const int buttonPins[NUM_BUTTONS] = {
  // Button, Down, Up, Left, Right
    5, 6, 7, 8, 9 // Player 1
};
const int outputPins[NUM_OUTPUTS] = {38, 39, 40, 41}; // Outputs

void setup() {
    Serial.begin(115200);

    for (int i = 0; i < NUM_BUTTONS; i++) {
        pinMode(buttonPins[i], INPUT); // Active-low buttons
    }

    for (int i = 0; i < NUM_OUTPUTS; i++) {
        pinMode(outputPins[i], OUTPUT);
        digitalWrite(outputPins[i], LOW); // Default outputs to OFF
    }
}

void loop() {
    // Read all 36 buttons (including joystick buttons)
    for (int i = 0; i < NUM_BUTTONS; i++) {
        Serial.print(digitalRead(buttonPins[i]));
        Serial.print(i < NUM_BUTTONS - 1 ? "," : "\n"); // Format as CSV
    }

    // Read Unity command for outputs
    if (Serial.available() > 0) {
        char command = Serial.read();
        if (command >= '0' && command <= '3') {
            int outputIndex = command - '0';
            digitalWrite(outputPins[outputIndex], !digitalRead(outputPins[outputIndex])); // Toggle output
        }
    }

    delay(5); // Prevent spam
}
