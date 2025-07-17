let showtimeIndex = 0;

function addShowtimeRow(date = "", time = "", room = "") {
    const container = document.getElementById("showtimesContainer");
    const row = document.createElement("div");
    row.className = "row mb-3 showtime-row";
    row.dataset.index = showtimeIndex;
    row.innerHTML = `
        <div class="col-md-3">
            <label class="fw-bold" style="margin-bottom: .5rem;">Ngày</label>
            <input type="date" class="form-control showtime-date" value="${date}" onchange="updateShowtimesJson()" />
        </div>
        <div class="col-md-3">
            <label class="fw-bold" style="margin-bottom: .5rem;">Giờ</label>
            <input type="time" class="form-control showtime-time" value="${time}" onchange="updateShowtimesJson()" />
        </div>
        <div class="col-md-4">
            <label class="fw-bold" style="margin-bottom: .5rem;">Rạp</label>
            <select class="form-select showtime-room" onchange="updateShowtimesJson()">
                ${window.allRooms.map(r => `<option value="${r}" ${r === room ? "selected" : ""}>${r}</option>`).join('')}
            </select>
        </div>
        <div class="col-md-2">
            <label class="form-label d-block" style="margin-bottom: .5rem;">&nbsp;</label>
            <button type="button"
                class="btn btn-sm btn-danger w-100 fw-bold"
                style="height: 38px;"
                onclick="removeShowtimeRow(this)"><i class="fa-solid fa-trash"></i> Xoá
            </button>
        </div>
    `;
    container.appendChild(row);
    showtimeIndex++;
    updateShowtimesJson();
}

function updateShowtimesJson() {
    const showtimeRows = document.querySelectorAll(".showtime-row");
    const showtimes = [];

    showtimeRows.forEach(row => {
        const date = row.querySelector(".showtime-date")?.value?.trim();
        const time = row.querySelector(".showtime-time")?.value?.trim();
        const roomName = row.querySelector(".showtime-room")?.value?.trim();

        if (date && time && roomName) {
            showtimes.push({ date, time, roomName });
        }
    });

    const hiddenInput = document.getElementById("ShowtimesJson");
    if (hiddenInput) hiddenInput.value = JSON.stringify(showtimes);
}

function removeShowtimeRow(button) {
    const row = button.closest(".showtime-row");
    if (row) {
        row.remove();
        updateShowtimesJson();
    }
}

function loadInitialShowtimes() {
    const rawJson = document.getElementById("ShowtimesJson")?.value;
    if (!rawJson || rawJson === "null" || rawJson.trim() === "") return;

    try {
        const showtimes = JSON.parse(rawJson);
        if (Array.isArray(showtimes)) {
            showtimes.forEach(item => {
                const date = (item.date || "").substring(0, 10);
                const time = (item.time || "").substring(0, 5);
                const room = item.roomName || "";
                addShowtimeRow(date, time, room);
            });
        }
    } catch (err) {
        console.error("Không thể parse JSON suất chiếu:", err);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadInitialShowtimes();
});