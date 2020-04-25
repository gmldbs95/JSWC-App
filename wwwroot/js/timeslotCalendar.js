console.log("test");
var oldList;
var lastClicked;
$(document).ready(getCalendar);
function getCalendar(){
  var theBody = {
    'PractitionerId': $('#practSelector').val(),
    'ServiceId': $('#servSelector').val(),
    'CustomerId': $('#custSelector').val(),
  }
  var theBodyString = JSON.stringify(theBody);
  console.log(theBodyString);
  $.ajax({
    method: "POST",
    url: "/calendarFilterJson",
    data: {'': theBodyString}
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
      $('#tsid').val(info.event.id);
      console.log(info.event.id);
      info.el.style.borderColor = "red";
      info.el.style.backgroundColor = "#FF0000";
    },
    editable  : false,
    droppable : false, // this allows things to be dropped onto the calendar !!!
  });

  calendar.render();
}
$('#custSelector').change(getCalendar);
$('#practSelector').change(getCalendar);
$('#servSelector').change(getCalendar);