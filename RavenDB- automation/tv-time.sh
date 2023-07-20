#!/bin/bash
export GET_TVSHOW_TOTAL_LENGTH_BIN="C:\Users\lieln\OneDrive\Desktop\RavenDB- automation\GetTvShowTotalLength\bin\Debug\net6.0\GetTvShowTotalLength.dll"
tv_show_file=$1

if [ ! -f "$GET_TVSHOW_TOTAL_LENGTH_BIN" ]; then
    echo "Error: C# application binary not found. Please set the GET_TVSHOW_TOTAL_LENGTH_BIN environment variable correctly."
    exit 1
fi

pids=()
exit_values=()
show_names=() # Array to store the names of the TV shows

# Read the TV show names from the file and process them in parallel
while IFS= read -r line || [[ -n "$line" ]]; do
    line=$(echo "$line" | tr -d '\r')

    if [ -z "$line" ]; then
        continue
    fi

    # Execute the C# application for each non-empty TV show in the background
    exit_value=$(dotnet "$GET_TVSHOW_TOTAL_LENGTH_BIN" "$line" "$$")
    if [ "$exit_value" -eq 10 ]; then
        echo "Could not get info for '$line'."
    else
        show_names+=("$line")
        pids+=($!)
        exit_values+=($exit_value)
    fi

done < "$tv_show_file"

# Wait for all background processes to finish
for pid in "${pids[@]}"; do
    wait "$pid"
done

# Find the shortest and longest exit values
shortest_value=${exit_values[0]}
longest_value=${exit_values[0]}
shortest_show=""
longest_show=""

for i in "${!exit_values[@]}"; do
    exit_value=${exit_values[$i]}
    show_name=${show_names[$i]}

    if [ "$exit_value" -lt "$shortest_value" ]; then
        shortest_value="$exit_value"
        shortest_show="$show_name"
    fi

    if [ "$exit_value" -gt "$longest_value" ]; then
        longest_value="$exit_value"
        longest_show="$show_name"
    fi
done

shortest_hours=$((shortest_value / 60))
shortest_minutes=$((shortest_value % 60))

longest_hours=$((longest_value / 60))
longest_minutes=$((longest_value % 60))

# Print the shortest and longest exit values along with their respective show names
echo "The shortest show: $shortest_show ("$shortest_hours"h "$shortest_minutes"m)"
echo "The longest show: $longest_show ("$longest_hours"h "$longest_minutes"m)"