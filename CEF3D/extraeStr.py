import os
import re
import sys
import keyword

# Uso:
#   python i18n_extract_replace.py <directorio_raiz> [salida.txt]

ROOT = sys.argv[1] if len(sys.argv) > 1 else "."
OUT_FILE = sys.argv[2] if len(sys.argv) > 2 else "strings.txt"

# Matches C# strings: "..." (con escapes)
STRING_REGEX = re.compile(r'"(?:\\.|[^"\\])*"')

# Detecta strings tipo solo placeholders: "{...}" o "{...}{...}"
ONLY_PLACEHOLDERS_REGEX = re.compile(r'^(?:\{[^{}]*\})+$')

def to_valid_identifier(text):
    # Quitar contenido de placeholders
    text = re.sub(r'\{[^{}]*\}', '', text)

    # Reemplazar no alfanumérico por _
    text = re.sub(r'[^0-9a-zA-Z]+', '_', text)

    # Trim _
    text = text.strip('_')

    # Si queda vacío
    if not text:
        text = "str"

    # Si empieza con número
    if text[0].isdigit():
        text = "str_" + text

    # Min longitud 3
    if len(text) < 3:
        text = text + "_str"

    # Evitar keywords de C#
    if keyword.iskeyword(text.lower()):
        text = text + "_val"

    return text

def extract_and_replace_file(path, collected, used_names):
    try:
        with open(path, "r", encoding="utf-8", errors="ignore") as f:
            content = f.read()

        def replacer(match):
            raw = match.group(0)
            value = raw[1:-1]  # sin comillas

            # omitir "{}" o similares
            if ONLY_PLACEHOLDERS_REGEX.match(value.strip()):
                return raw

            var_name = to_valid_identifier(value)

            # evitar colisiones
            base = var_name
            i = 1
            while var_name in used_names:
                var_name = f"{base}_{i}"
                i += 1

            used_names.add(var_name)
            collected[var_name] = value

            return f'I18n.{var_name}'

        new_content = STRING_REGEX.sub(replacer, content)

        if new_content != content:
            with open(path, "w", encoding="utf-8") as f:
                f.write(new_content)

    except Exception:
        pass

def walk(root):
    collected = {}
    used_names = set()

    for base, _, files in os.walk(root):
        for file in files:
            if file.endswith(".cs"):
                full_path = os.path.join(base, file)
                extract_and_replace_file(full_path, collected, used_names)

    return collected

def main():
    collected = walk(ROOT)

    with open(OUT_FILE, "w", encoding="utf-8") as f:
        for k in sorted(collected.keys()):
            f.write(f"{k}={collected[k]}\n")

    print(f"{len(collected)} strings extraídos y reemplazados.")

if __name__ == "__main__":
    main()