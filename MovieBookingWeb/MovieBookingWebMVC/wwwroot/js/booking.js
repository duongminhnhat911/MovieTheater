const seatPrice = 95000;
const seatContainer = document.getElementById("seatContainer");
const timeSlotsDiv = document.getElementById("time-list");
const selectedSeatsInput = document.getElementById("SelectedSeats");
const selectedDateInput = document.getElementById("SelectedDate");
const selectedTimeInput = document.getElementById("SelectedTime");
const selectedRoomInput = document.getElementById("SelectedRoom");
const roomDisplay = document.getElementById("selected-room-name");
const totalAmountSpan = document.getElementById("totalAmount");
const summaryDiv = document.getElementById("booking-summary");
const countdownSpan = document.getElementById("countdown");

let countdownInterval;
let selectedDate = "";
let selectedTime = "";
let selectedRoom = "";
let selected = [];

document.querySelectorAll(".date-btn").forEach(btn => {
    btn.addEventListener("click", function () {
        selectedDate = this.dataset.date;
        selectedTime = "";
        selectedRoom = "";
        selectedDateInput.value = selectedDate;

        updateTimeSlots();
        resetBookingState();
        document.getElementById("time-section").classList.remove("d-none");
    });
});

function updateTimeSlots() {
    timeSlotsDiv.innerHTML = "";

    if (!window.roomByDateTime[selectedDate]) {
        console.warn("Không có giờ chiếu cho ngày:", selectedDate);
        return;
    }

    Object.keys(window.roomByDateTime[selectedDate]).forEach(time => {
        const btn = document.createElement("button");
        btn.type = "button";
        btn.innerText = time;
        btn.classList.add("btn", "btn-secondary", "m-1");
        btn.addEventListener("click", function () {
            selectedTime = time;
            selectedRoom = window.roomByDateTime[selectedDate][time];
            selectedTimeInput.value = selectedTime;
            selectedRoomInput.value = selectedRoom;
            roomDisplay.innerText = selectedRoom;
            updateSeats();
            document.getElementById("seat-section").classList.remove("d-none");
        });

        timeSlotsDiv.appendChild(btn);
    });
}

function updateSeats() {
    seatContainer.innerHTML = "";
    selected = [];
    selectedSeatsInput.value = "";
    totalAmountSpan.innerText = "0";
    summaryDiv.classList.add("d-none");

    const roomLayout = window.roomLayouts?.[selectedRoom];
    if (!roomLayout) {
        console.warn("Không tìm thấy layout của phòng:", selectedRoom);
        return;
    }

    const rowCount = roomLayout.rows;
    const colCount = roomLayout.columns;
    const rows = Array.from({ length: rowCount }, (_, i) => String.fromCharCode(65 + i));
    const booked = (window.bookedSeats?.[selectedDate]?.[selectedTime]) || [];
    const table = document.createElement("table");

    table.style.borderCollapse = "separate";
    table.style.borderSpacing = "5px 5px";
    table.style.margin = "0 auto";
    table.style.tableLayout = "fixed";

    rows.forEach(row => {
        const tr = document.createElement("tr");
        const rowLabel = document.createElement("td");
        rowLabel.innerText = row;
        rowLabel.style.fontWeight = "bold";
        rowLabel.style.padding = "2px 12px 2px 6px";
        rowLabel.style.border = "none";
        tr.appendChild(rowLabel);
        for (let col = 1; col <= colCount; col++) {
            if (col === 4) {
                const aisleTd = document.createElement("td");
                aisleTd.style.border = "none";
                aisleTd.style.width = "20px";
                tr.appendChild(aisleTd);
            }
            const seatId = row + col;
            const td = document.createElement("td");
            td.style.padding = "2px";
            const btn = createSeatButton(seatId, booked);
            td.appendChild(btn);
            tr.appendChild(td);
        }
        table.appendChild(tr);
    });
    seatContainer.appendChild(table);
    resizeScreenImage();
}

function createSeatButton(seatId, booked) {
    const btn = document.createElement("button");
    btn.innerText = seatId;
    btn.setAttribute("type", "button");
    btn.classList.add("btn", "seat-btn");

    if (booked.includes(seatId)) {
        btn.disabled = true;
        btn.classList.add("btn-secondary");
    } else {
        btn.classList.add("btn-outline-primary");

        btn.addEventListener("click", function () {
            if (selected.includes(seatId)) {
                selected = selected.filter(s => s !== seatId);
                btn.classList.remove("btn-primary");
                btn.classList.add("btn-outline-primary");
            
            } else {
                selected.push(seatId);
                btn.classList.remove("btn-outline-primary");
                btn.classList.add("btn-primary");
            }
            selectedSeatsInput.value = selected.join(",");
            totalAmountSpan.innerText = new Intl.NumberFormat('vi-VN').format(selected.length * seatPrice);
            
            if (selected.length > 0) {
                summaryDiv.classList.remove("d-none");
                document.getElementById("submit-btn").classList.remove("d-none");
                if (!countdownInterval) startCountdown(5 * 60);
            
            } else {
                summaryDiv.classList.add("d-none");
                document.getElementById("submit-btn").classList.add("d-none");
                clearInterval(countdownInterval);
                countdownInterval = null;
                countdownSpan.innerText = "05:00";
            }
        });
    }
    return btn;
}

function resizeScreenImage() {
    requestAnimationFrame(() => {
        const seatTable = document.querySelector("#seatContainer table");
        const screenImg = document.getElementById("screen-img");
        if (seatTable && screenImg) {
            const seatWidth = seatTable.offsetWidth;
            const maxWidth = 600;
            const finalWidth = Math.min(seatWidth, maxWidth);
            const naturalWidth = screenImg.naturalWidth;
            const naturalHeight = screenImg.naturalHeight;
            if (naturalWidth && naturalHeight) {
                const ratio = naturalHeight / naturalWidth;
                screenImg.style.width = finalWidth + "px";
                screenImg.style.height = (finalWidth * ratio) + "px";
            } else {
                screenImg.style.width = finalWidth + "px";
                screenImg.style.height = "auto";
            }
        }
    });
}

function startCountdown(duration) {
    let timer = duration;
    countdownInterval = setInterval(function () {
        const minutes = Math.floor(timer / 60);
        const seconds = timer % 60;
        
        countdownSpan.textContent = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        
        if (--timer < 0) {
            clearInterval(countdownInterval);
            countdownInterval = null;
            alert("Hết thời gian giữ ghế. Vui lòng chọn lại.");
            document.querySelectorAll("#seatContainer button.btn-primary").forEach(btn => btn.click());
            summaryDiv.classList.add("d-none");
            countdownSpan.textContent = "05:00";
        }
    }, 1000);
}

function resetBookingState() {
    document.getElementById("seat-section").classList.add("d-none");
    document.getElementById("selected-room-name").innerText = "";
    seatContainer.innerHTML = "";
    summaryDiv.classList.add("d-none");
    selectedSeatsInput.value = "";
    totalAmountSpan.innerText = "0";
    document.getElementById("submit-btn").classList.add("d-none");
    clearInterval(countdownInterval);
    countdownInterval = null;
    countdownSpan.innerText = "05:00";
}

document.addEventListener('DOMContentLoaded', () => {
    const bookingSummary = document.getElementById('booking-summary');
    const footer = document.querySelector('footer');

    if (!bookingSummary || !footer) return;

    function updateBookingSummaryPosition() {
        if (bookingSummary.offsetParent === null) {
            return;
        }  
        const footerRect = footer.getBoundingClientRect();
        const bookingHeight = bookingSummary.offsetHeight;
        const windowHeight = window.innerHeight;
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        const footerTopOnPage = scrollTop + footerRect.top;
        const bookingTopWhenStickyStop = footerTopOnPage - bookingHeight;

        if (footerRect.top < windowHeight) {
            bookingSummary.classList.add('sticky-stop');
            bookingSummary.style.top = `${bookingTopWhenStickyStop}px`;
            bookingSummary.style.bottom = 'auto';
        } else {
            bookingSummary.classList.remove('sticky-stop');
            bookingSummary.style.top = 'auto';
            bookingSummary.style.bottom = '0';
        }
    }

    window.addEventListener('scroll', updateBookingSummaryPosition);
    window.addEventListener('resize', updateBookingSummaryPosition);

    updateBookingSummaryPosition();
});
