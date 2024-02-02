var _current_field = null;

function prepareLoginPage(){
    $('#txtNoKartu').on('click',function(ev){
        $('#txtNoKartu').addClass('active');
        $('#txtPin').removeClass('active');
    });
    $('#txtPin').on('click',function(ev){
        $('#txtPin').addClass('active');
        $('#txtNoKartu').removeClass('active');
    });
    $('.keypad>.key').each(function(i,o){
        var k = $(o);
        k.on('click',function(ev){
            var src = $(ev.target);
            var v = src.attr('data-key');
            var act = null;
            if ($('#txtPin').is('.active')) {act = $('#txtPin');}
            else {act = $('#txtNoKartu').addClass('active');}
            switch(v) {
                case 'E' :
                    if ($('#txtNoKartu').is('.active')) {$('#txtPin').trigger('click');}
                    else {$('#txtNoKartu').trigger('click');}
                    break;
                case 'D' :
                    var val = act.next().val();
                    var nval = val.substr(0,val.length-1)
                    act.text(nval);
                    act.next().val(nval);
                    break;
                default :
                    var val = act.next().val();
                    var nval = val+v;
                    if (act.is('.mask')) {
                        var ctr = val.length;
                        act.text('*'.repeat(ctr)+v);
                    } else  act.text(nval);
                    act.next().val(nval);
                    break;       
            }
            var vto = null;
            if (act.is('.mask')) {
                clearTimeout(vto);
                vto = setTimeout(function(){
                    var ctr = act.next().val().length;
                    act.text('*'.repeat(ctr));
                },500);
            }
        });
        

    });
}

$(function(){
    // init
    prepareLoginPage();
});