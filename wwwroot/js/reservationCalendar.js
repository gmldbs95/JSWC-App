var oldList;
var lastClicked;
$('#calendarTabID').on('click', getCalendar);
function getCalendar(){
  $.ajax({
    method: "GET",
    url: "/calendarReservationsJson"
    }).done(function(res){
      oldList = JSON.parse(res);
      console.log(oldList);
      makeCalendar(oldList);
    })
}
function makeCalendar(myEventList){
  if (document.getElementById("calendar").innerHTML != "") {
    document.getElementById("calendar").innerHTML = "";
  }
  /* initialize the calendar
  -----------------------------------------------------------------*/


  var Calendar = FullCalendar.Calendar;
  var calendarEl = document.getElementById('calendar');
  var calendar = new Calendar(calendarEl, {
    timeZone: 'UTC',
    plugins: [ 'bootstrap', 'interaction', 'dayGrid', 'timeGrid' ],
    header    : {
      left  : 'prev,next today',
      center: 'title',
      right : 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    //Random default events
    events    : myEventList.result,
    eventClick: function(info){
      document.location.href = `/rec/CurrentReservation/${info.event.id}`;
    },
    editable  : false,
    droppable : false, // this allows things to be dropped onto the calendar !!!
  });

  calendar.render();
}